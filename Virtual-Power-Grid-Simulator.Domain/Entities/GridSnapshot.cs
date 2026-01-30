using System;
using System.ComponentModel.DataAnnotations;
using Virtual_Power_Grid_Simulator.Domain.Enums;

namespace Virtual_Power_Grid_Simulator.Domain.Entities;

public class GridSnapshot
{
    public Guid Id { get; init; }
    public DateTime TimeStamp { get; init; }
    public decimal TotalGeneration { get; init; }
    public decimal TotalDemand { get; init; }
    public decimal Frequency { get; init; }
    public GridStatus Status { get; init; }

    public GridSnapshot(DateTime time, IEnumerable<PowerPlant> powerPlants, IEnumerable<PowerConsumer> powerConsumers)
    {
        Id = Guid.NewGuid();
        TimeStamp = time;
        TotalGeneration = CalculateTotalGeneration(powerPlants);
        TotalDemand = CalculateTotalDemand(powerConsumers, time);
        Frequency = CalculateFrequency(demand: TotalDemand, generation: TotalGeneration);
        Status = CalculateGridStatus(Frequency);
    }

    private decimal CalculateTotalGeneration(IEnumerable<PowerPlant> powerPlants)
    {
        return powerPlants
            .Where(pp => pp.IsWorking)
            .Sum(pp => pp.CurrentPower);
    }

    private decimal CalculateTotalDemand(IEnumerable<PowerConsumer> powerConsumers, DateTime time)
    {
        return powerConsumers
            .Where(pc => pc.IsActive)
            .Sum(pc => pc.CalculateConsumption(time));
    }

    private decimal CalculateFrequency(decimal demand, decimal generation)
    {
        return 50 + (generation - demand) * 0.05m;
    }

    private GridStatus CalculateGridStatus(decimal frequency)
    {
        return frequency switch
        {
            >= 49.8m and <= 50.2m => GridStatus.Normal,
            (>= 49.5m and < 49.8m) or (> 50.2m and <= 50.5m ) => GridStatus.Warning,
            (>= 49.0m and < 49.5m) or (> 50.5m and <= 51.0m) => GridStatus.Critical,
            _ => GridStatus.Blackout
        };
    }
}
