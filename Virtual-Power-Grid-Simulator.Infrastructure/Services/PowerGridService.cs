using System;
using Microsoft.Extensions.Logging;
using Virtual_Power_Grid_Simulator.Application.Interfaces;
using Virtual_Power_Grid_Simulator.Domain.Entities;
using Virtual_Power_Grid_Simulator.Infrastructure.Repositories;

namespace Virtual_Power_Grid_Simulator.Infrastructure.Services;

public class PowerGridService : IPowerGridService
{
    private readonly ILogger<PowerGridService> _logger;
    private readonly PowerPlantRepository _plantRepo;
    private readonly PowerConsumerRepository _consumerRepo;
    private static DateTime _simulationTime = new DateTime(2026, 1, 1, 8, 0, 0);

    public PowerGridService(ILogger<PowerGridService> logger, PowerPlantRepository plantRepo, PowerConsumerRepository consumerRepo)
    {
        _logger = logger;
        _plantRepo = plantRepo;
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

    public void UpdateSimulationTime()
    {
        _simulationTime = _simulationTime.AddMinutes(12);

        var plants = _plantRepo.GetAll();
        
        foreach (var plant in plants)
        {
            plant.Tick();
            _plantRepo.Update(plant);
        }
    }
}
