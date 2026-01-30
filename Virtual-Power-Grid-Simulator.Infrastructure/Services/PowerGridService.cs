using System;
using Microsoft.Extensions.Logging;
using Virtual_Power_Grid_Simulator.Application.Interfaces;
using Virtual_Power_Grid_Simulator.Domain.Entities;

namespace Virtual_Power_Grid_Simulator.Infrastructure.Services;

public class PowerGridService : IPowerGridService
{
    private readonly ILogger<PowerGridService> _logger;
    private readonly List<PowerPlant> _powerPlants = new();
    private readonly List<PowerConsumer> _consumers = new();
    private DateTime _simulationTime = new DateTime(2026, 1, 1, 8, 0, 0);

    public PowerGridService(ILogger<PowerGridService> logger)
    {
        _logger = logger;
    }
    public void AdjustPowerPlant(Guid id, decimal targetPower)
    {
        var plant = GetPowerPlantById(id);
        plant.AdjustPower(targetPower, 1.0);
    }
    public GridSnapshot CalculateGridState()
    {
        GridSnapshot snapshot = new GridSnapshot(_simulationTime, _powerPlants, _consumers);
        return snapshot;
    }

    public void ConnectConsumer(Guid consumerId)
    {
        var consumer = GetConsumerById(consumerId);
        consumer.Connect();
    }

    public void DisconnectConsumer(Guid consumerId)
    {
        var consumer = GetConsumerById(consumerId);
        consumer.Disconnect();
    }

    public IEnumerable<PowerConsumer> GetAllConsumers()
    {
        return _consumers.ToList();
    }

    public IEnumerable<PowerPlant> GetAllPowerPlants()
    {
        return _powerPlants.ToList();
    }

    public PowerConsumer GetConsumerById(Guid id)
    {
        PowerConsumer? consumer = _consumers.FirstOrDefault(c => c.Id == id);
        return consumer ?? throw new KeyNotFoundException($"Consumer with ID {id} not found.");
    }

    public PowerPlant GetPowerPlantById(Guid id)
    {
        PowerPlant? plant = _powerPlants.FirstOrDefault(p => p.Id == id);
        return plant ?? throw new KeyNotFoundException($"PowerPlant with ID {id} not found.");
    }

    public Guid RegisterConsumer(PowerConsumer consumer)
    {
        if(_consumers.Any(c => c.Id == consumer.Id))
        {
            throw new InvalidOperationException($"Consumer with ID {consumer.Id} is already registered.");
        }
        _consumers.Add(consumer);
        return consumer.Id;
    }

    public Guid RegisterPowerPlant(PowerPlant powerPlant)
    {
        if(_powerPlants.Any(p => p.Id == powerPlant.Id))
        {
            throw new InvalidOperationException($"PowerPlant with ID {powerPlant.Id} is already registered.");
        }
        _powerPlants.Add(powerPlant);
        return powerPlant.Id;
    }

    public void TurnOffPowerPlant(Guid id)
    {
        var plant = GetPowerPlantById(id);
        plant.TurnOff();
    }

    public void TurnOnPowerPlant(Guid id)
    {
        var plant = GetPowerPlantById(id);
        plant.TurnOn();
    }

    public void UpdateSimulationTime()
    {
        _simulationTime = _simulationTime.AddMinutes(12);
        foreach( var plant in _powerPlants)
        {
            plant.Tick();
        }
    }
}
