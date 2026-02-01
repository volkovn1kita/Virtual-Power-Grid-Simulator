using System;
using Microsoft.Extensions.Logging;
using Virtual_Power_Grid_Simulator.Application.Interfaces;
using Virtual_Power_Grid_Simulator.Domain.Entities;
using Virtual_Power_Grid_Simulator.Domain.Enums;
using Virtual_Power_Grid_Simulator.Infrastructure.Repositories;

namespace Virtual_Power_Grid_Simulator.Infrastructure.Services;

public class PowerGridService : IPowerGridService
{
    private readonly ILogger<PowerGridService> _logger;
    private readonly PowerPlantRepository _plantRepo;
    private readonly IWeatherService _weatherService;
    private readonly PowerConsumerRepository _consumerRepo;
    private static DateTime _simulationTime = new DateTime(2026, 1, 1, 8, 0, 0);

    public PowerGridService(ILogger<PowerGridService> logger, PowerPlantRepository plantRepo, IWeatherService weatherService, PowerConsumerRepository consumerRepo)
    {
        _logger = logger;
        _plantRepo = plantRepo;
        _weatherService = weatherService;
        _consumerRepo = consumerRepo;
    }
    public void AdjustPowerPlant(Guid id, decimal targetPower)
    {
        var plant = _plantRepo.GetById(id) 
                    ?? throw new KeyNotFoundException("Plant not found");
        
        plant.AdjustPower(targetPower);
        
        _plantRepo.Update(plant);
    }
    public GridSnapshot CalculateGridState()
    {
        GridSnapshot snapshot = new GridSnapshot(_simulationTime, _plantRepo.GetAll(), _consumerRepo.GetAll());
        return snapshot;
    }

    public void ConnectConsumer(Guid consumerId)
    {
        var consumer = GetConsumerById(consumerId);
        consumer.Connect();
        _consumerRepo.Update(consumer);
    }

    public void DisconnectConsumer(Guid consumerId)
    {
        var consumer = GetConsumerById(consumerId);
        consumer.Disconnect();
        _consumerRepo.Update(consumer);
    }

    public IEnumerable<PowerConsumer> GetAllConsumers()
    {
        return _consumerRepo.GetAll();
    }

    public IEnumerable<PowerPlant> GetAllPowerPlants()
    {
        return _plantRepo.GetAll();
    }

    public PowerConsumer GetConsumerById(Guid id)
    {
        PowerConsumer? consumer = _consumerRepo.GetById(id);
        return consumer ?? throw new KeyNotFoundException($"Consumer with ID {id} not found.");
    }

    public PowerPlant GetPowerPlantById(Guid id)
    {
        PowerPlant? plant = _plantRepo.GetById(id);
        return plant ?? throw new KeyNotFoundException($"PowerPlant with ID {id} not found.");
    }

    public Guid RegisterConsumer(PowerConsumer consumer)
    {
        _consumerRepo.Add(consumer);
        return consumer.Id;
    }

    public Guid RegisterPowerPlant(PowerPlant powerPlant)
    {
        _plantRepo.Add(powerPlant);
        return powerPlant.Id;
    }

    public void TurnOffPowerPlant(Guid id)
    {
        var plant = GetPowerPlantById(id);
        plant.TurnOff();
        _plantRepo.Update(plant);
    }

    public void TurnOnPowerPlant(Guid id)
    {
        var plant = GetPowerPlantById(id);
        plant.TurnOn();
        _plantRepo.Update(plant);
    }

    public async Task UpdateSimulationTimeAsync()
    {
        _simulationTime = _simulationTime.AddMinutes(12);

        var weather = await _weatherService.GetCurrentWeatherAsync();

        double timeOfDay = _simulationTime.Hour + (_simulationTime.Minute / 60.0);
        double sunFactor = 0.0;

        if (timeOfDay >= 6 && timeOfDay <= 20)
        {
            sunFactor = Math.Sin((timeOfDay - 6) * Math.PI / 14.0);
        }

        double solarEfficiency = sunFactor * weather.CloudinessFactor;

        double maxWindSpeed = 20.0; 
        double windEfficiency = weather.WindSpeed / maxWindSpeed;
        if (windEfficiency > 1.0) windEfficiency = 1.0;

        var plants = _plantRepo.GetAll();
        
        foreach (var plant in plants)
        {
            double efficiency = 1.0;

            if (plant.Type == PowerPlantType.Solar)
            {
                efficiency = solarEfficiency;
            }
            else if (plant.Type == PowerPlantType.Wind)
            {
                efficiency = windEfficiency;
            }
            plant.Tick(efficiency);
            
            _plantRepo.Update(plant);
        }
        var consumers = _consumerRepo.GetAll();
        ManageGridStability(plants, consumers);
    }
    
    private void ManageGridStability(List<PowerPlant> plants, List<PowerConsumer> consumers)
    {

        decimal frequency = CalculationFrequency(plants, consumers);

        if (frequency <= 49.5m)
        {
            DeficitElectricity(plants, consumers);
        }
        else if (frequency > 50.0m)
        {
            ExcessElectricity(plants, consumers);
        }
    }

    private decimal CalculationFrequency(List<PowerPlant> plants, List<PowerConsumer> consumers)
    {
        decimal totalGeneration = plants.Where(p => p.IsWorking).Sum(p => p.CurrentPower);
        decimal totalDemand = consumers.Where(c => c.IsActive).Sum(c => c.CalculateConsumption(_simulationTime));

        decimal frequency = 50 + (totalGeneration - totalDemand) * 0.1m;
        return  frequency;
    }

    private void DeficitElectricity(List<PowerPlant> plants, List<PowerConsumer> consumers)
    {
        var candidatesToCut = consumers
            .Where(c => c.IsActive)
            .OrderBy(c => c.Priority)
            .ToList();
        
        foreach (var consumer in candidatesToCut)
        {
            consumer.Disconnect();
            _consumerRepo.Update(consumer);
            _logger.LogWarning($"[AFLS] Disconnected {consumer.Name} (Priority {consumer.Priority})");

            decimal newFreq = CalculationFrequency(plants, consumers);
            if (newFreq > 49.5m) return;
        }

        _logger.LogCritical("Total Blackout imminent - Load Shedding failed!");
    }

    private void ExcessElectricity(List<PowerPlant> plants, List<PowerConsumer> consumers)
    {
        var candidatesToRestore = consumers
            .Where(c => !c.IsActive)
            .OrderByDescending(c => c.Priority)
            .ToList();

        foreach (var consumer in candidatesToRestore)
        {
            decimal predictedLoad = consumer.CalculateConsumption(_simulationTime);
            
            decimal currentGen = plants.Where(p => p.IsWorking).Sum(p => p.CurrentPower);
            decimal currentDemand = consumers.Where(c => c.IsActive).Sum(c => c.CalculateConsumption(_simulationTime));
            
            decimal predictedFreq = 50 + (currentGen - (currentDemand + predictedLoad)) * 0.1m;

            if (predictedFreq >= 49.8m)
            {
                consumer.Connect();
                _consumerRepo.Update(consumer);
                _logger.LogInformation($"[Restoration] Connected {consumer.Name}. Freq will be: {predictedFreq:F2}");
            }
            else
            {
                break; 
            }
        }
    }
}
