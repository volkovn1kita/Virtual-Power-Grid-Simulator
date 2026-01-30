using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using Virtual_Power_Grid_Simulator.Domain.Enums;

namespace Virtual_Power_Grid_Simulator.Domain.Entities;

public class PowerConsumer
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public ConsumerType Type { get; private set; }
    public decimal MaxPeakLoad { get; private set; }
    public bool IsActive { get; private set; } = true;

    [Range(1,10)]
    public int Priority { get; private set; }
    
    public PowerConsumer(string name, ConsumerType type, decimal maxPeakLoad, int priority)
    {
        if (maxPeakLoad <= 0)
            throw new ArgumentException("Max peak load should be positive");
        
        
        if (priority < 1 || priority > 10)
            throw new ArgumentException("Priority should be in range from 1 to 10");

        
        Id = Guid.NewGuid();
        Name = name;
        Type = type;
        MaxPeakLoad = maxPeakLoad;
        Priority = priority;
    }

    public void Connect()
    {
        IsActive = true;
    }
    public void Disconnect()
    {
        IsActive = false;
    }

    public decimal CalculateConsumption(DateTime simulationTime)
    {
        if (!IsActive) return 0;

        int currentHour = simulationTime.Hour;

        return MaxPeakLoad * GetUsageCoefficient(currentHour);
    }

    private decimal GetUsageCoefficient(int hour)
    {
        if (Type == ConsumerType.Residential)
        {
            if (6 <= hour && hour <= 9 ) return 1.0m;
            
            if (18 <= hour && hour <= 23 ) return 1.0m;
            
            if (0 <= hour && hour <= 5) return 0.2m;
            
            return 0.5m;
        }
        
        if (Type == ConsumerType.Industrial)
        {
            if (8 <= hour && hour <= 18) return 1.0m;

            return 0.1m;
        }

        return 1.0m;

    }
}
