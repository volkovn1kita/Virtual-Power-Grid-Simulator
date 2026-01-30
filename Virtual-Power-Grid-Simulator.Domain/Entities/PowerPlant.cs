using System;
using Virtual_Power_Grid_Simulator.Domain.Enums;
using Virtual_Power_Grid_Simulator.Domain.ValueObjects;

namespace Virtual_Power_Grid_Simulator.Domain.Entities;

public class PowerPlant
{
    // DEFAULT PROPS
    public Guid Id { get; } 
    public string Name { get; private set; }
    public PowerPlantType Type {get; private set;}

    // GENERATION STATUS
    public decimal CurrentPower { get; private set; }
    public bool IsWorking { get; private set; } = false;

    // TECHNICAL LIMITATIONS
    public decimal MaxCapacity { get; private set; }
    public decimal MinStableLoad { get; private set; }
    public decimal SetPoint { get; private set; }

    // DYNAMICS
    public decimal RampRatePerTick { get; private set; }

    // For renewable sources
    public bool IsVariableRenewable =>
        Type == PowerPlantType.Solar ||
        Type == PowerPlantType.Wind;
    
    public PowerPlant(
        string name,
        PowerPlantType type,
        decimal maxCapacity,
        decimal minStableLoad,
        decimal rampRate
        )
    {
        if (minStableLoad > maxCapacity)
            throw new ArgumentException("Min stable load might be bigger than max capacity");
        
        if (maxCapacity <= 0)
            throw new ArgumentException("Capacity must be positive");

        if (minStableLoad < 0)
            throw new ArgumentException("Min stable load must be 0 or positive");

        // SET VALUES
        Id = Guid.NewGuid();
        Name = name;
        Type = type;
        MaxCapacity = maxCapacity;
        MinStableLoad = minStableLoad;
        RampRatePerTick = rampRate;
        SetPoint = minStableLoad;

    }

    public void AdjustPower(decimal targetPower = 0, double weatherFactor = 1.0)
    {
        if (targetPower > MaxCapacity) targetPower = MaxCapacity;
        if (targetPower < MinStableLoad) targetPower = MinStableLoad;

        SetPoint = targetPower;
    }

    public void Tick()
    {
        if (!IsWorking) return;

        if (CurrentPower < SetPoint)
        {
            CurrentPower += Math.Min(SetPoint - CurrentPower, RampRatePerTick);
        }
        else if (CurrentPower > SetPoint)
        {
            CurrentPower -= Math.Min(CurrentPower - SetPoint, RampRatePerTick);
        }
    }

    public void TurnOn()
    {
        IsWorking = true;
        CurrentPower = MinStableLoad;
    }
   
    public void TurnOff()
    {
        IsWorking = false;
        CurrentPower = 0;
    }
}
