using System;
using Virtual_Power_Grid_Simulator.Domain.Entities;
using Virtual_Power_Grid_Simulator.Domain.Enums;

namespace Virtual_Power_Grid_Simulator.Tests.Domain;

public class GridSnapshotTests
{
    
    [Fact]
    void Calculation_BalanceIsPerfect_FrequencyShouldBe50 ()
    {
        var pp = new PowerPlant("NPP", PowerPlantType.Nuclear, 100, 90, 20);
        pp.TurnOn();
        pp.AdjustPower(100);
        var powerPlants = new List<PowerPlant>{pp};

        var cp = new PowerConsumer("Factory", ConsumerType.Industrial, 100, 2);
        var powerConsumers = new List<PowerConsumer>{cp};

        var snapshot = new GridSnapshot(new DateTime(2025, 1, 1, 12, 0, 0), powerPlants, powerConsumers);
  
        Assert.Equal(50m, snapshot.Frequency);
        Assert.Equal(GridStatus.Normal,snapshot.Status);
    }

    [Fact]
    void Calculation_Deficit_FrequencyShouldDrop()
    {
        var pp = new PowerPlant("NPP", PowerPlantType.Nuclear, 100, 90, 20);
        pp.TurnOn();
        pp.AdjustPower(100);
        var powerPlants = new List<PowerPlant>{pp};

        var cp = new PowerConsumer("Factory", ConsumerType.Industrial, 120, 2);
        var powerConsumers = new List<PowerConsumer>{cp};

        var snapshot = new GridSnapshot(new DateTime(2025, 1, 1, 12, 0, 0), powerPlants, powerConsumers);

        Assert.Equal(50m + (100 - 120) * 0.05m, snapshot.Frequency);
        Assert.Equal(GridStatus.Critical, snapshot.Status);
    }

    [Fact]
    void Calculation_IgnoreInactiveConsumers()
    {
        var pp = new PowerPlant("NPP", PowerPlantType.Nuclear, 100, 90, 20);
        pp.TurnOn();
        pp.AdjustPower(100);
        var powerPlants = new List<PowerPlant>{pp};

        var cp = new PowerConsumer("Factory", ConsumerType.Industrial, 500, 2);
        cp.Disconnect();
        var powerConsumers = new List<PowerConsumer>{cp};

        var snapshot = new GridSnapshot(new DateTime(2025, 1, 1, 12, 0, 0), powerPlants, powerConsumers);

        Assert.Equal(50m + (100 - 0) * 0.05m, snapshot.Frequency);
        Assert.Equal(GridStatus.Blackout, snapshot.Status);
    }

    [Fact]
    void Calculation_IgnoreInactivePlants()
    {
        var pp = new PowerPlant("NPP", PowerPlantType.Nuclear, 100, 90, 20);
        pp.TurnOn();
        pp.AdjustPower(100);
        pp.TurnOff();
        var powerPlants = new List<PowerPlant>{pp};

        var cp = new PowerConsumer("Factory", ConsumerType.Industrial, 100, 2);
        var powerConsumers = new List<PowerConsumer>{cp};

        var snapshot = new GridSnapshot(new DateTime(2025, 1, 1, 12, 0, 0), powerPlants, powerConsumers);

        Assert.Equal(50m + (0 - 100) * 0.05m, snapshot.Frequency);
        Assert.Equal(GridStatus.Blackout, snapshot.Status);
    }

    [Fact]
    void Calculation_EmptyLists_FrequencyShouldBe50 ()
    {
        var powerPlants = new List<PowerPlant>{};
        var powerConsumers = new List<PowerConsumer>{};

        var snapshot = new GridSnapshot(new DateTime(2025, 1, 1, 12, 0, 0), powerPlants, powerConsumers);
  
        Assert.Equal(50m, snapshot.Frequency);
        Assert.Equal(GridStatus.Normal,snapshot.Status);
    }

}
