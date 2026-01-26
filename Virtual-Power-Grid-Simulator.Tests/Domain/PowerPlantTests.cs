using System;
using Virtual_Power_Grid_Simulator.Domain.Entities;
using Virtual_Power_Grid_Simulator.Domain.Enums;

namespace Virtual_Power_Grid_Simulator.Tests.Domain;

public class PowerPlantTests
{
        
    [Fact]
    void Constructor_ValidParameters_ShouldCreatePowerPlant ()
    {
        string name = "KNPP";
        PowerPlantType type = PowerPlantType.Nuclear;
        decimal maxCapacity = 100;
        decimal minStableLoad = 40;
        decimal rampRate = 10;

        PowerPlant powerPlant = new PowerPlant(name, type, maxCapacity, minStableLoad, rampRate);

        Assert.Equal(name, powerPlant.Name);
        Assert.Equal(type, powerPlant.Type );
        Assert.Equal(maxCapacity, powerPlant.MaxCapacity);
        Assert.Equal(minStableLoad, powerPlant.MinStableLoad);
        Assert.Equal(rampRate, powerPlant.RampRatePerTick);
    }

    [Fact]
    void Constructor_NegativeCapacity_ShouldThrowException()
    {
        string name = "KNPP";
        PowerPlantType type = PowerPlantType.Nuclear;
        decimal maxCapacity = -100;
        decimal minStableLoad = 40;
        decimal rampRate = 10;

        
        Assert.Throws<ArgumentException>( ()=>
        {
            PowerPlant powerPlant = new PowerPlant(name, type, maxCapacity, minStableLoad, rampRate);
        });
    }

    [Fact]
    void AdjustPower_RampRateLogic_ShouldIncreaseByRampRate()
    {
        string name = "KNPP";
        PowerPlantType type = PowerPlantType.Nuclear;
        decimal maxCapacity = 100;
        decimal minStableLoad = 40;
        decimal rampRate = 10;
        PowerPlant powerPlant = new PowerPlant(name, type, maxCapacity, minStableLoad, rampRate);

        powerPlant.TurnOn();
        powerPlant.AdjustPower(100);

        Assert.Equal(50, powerPlant.CurrentPower);
    }

    [Fact]
    void TurnOn_ShouldSetPowerToMinStableLoad()
    {
        string name = "KNPP";
        PowerPlantType type = PowerPlantType.Nuclear;
        decimal maxCapacity = 100;
        decimal minStableLoad = 40;
        decimal rampRate = 10;
        PowerPlant powerPlant = new PowerPlant(name, type, maxCapacity, minStableLoad, rampRate);
        
        powerPlant.TurnOn();

        Assert.Equal(40, powerPlant.CurrentPower);
        Assert.True(powerPlant.IsWorking);
    }

    [Fact]
    void TurnOff_ShouldSetPowerToZero()
    {
        string name = "KNPP";
        PowerPlantType type = PowerPlantType.Nuclear;
        decimal maxCapacity = 100;
        decimal minStableLoad = 40;
        decimal rampRate = 10;
        PowerPlant powerPlant = new PowerPlant(name, type, maxCapacity, minStableLoad, rampRate);
        
        powerPlant.TurnOff();

        Assert.Equal(0, powerPlant.CurrentPower);
        Assert.False(powerPlant.IsWorking);
    }

    [Fact]
    void AdjustPower_MaxCapacity_ShouldReturnMaxCapacity()
    {
        string name = "KNPP";
        PowerPlantType type = PowerPlantType.Nuclear;
        decimal maxCapacity = 100;
        decimal minStableLoad = 95;
        decimal rampRate = 10;
        PowerPlant powerPlant = new PowerPlant(name, type, maxCapacity, minStableLoad, rampRate);
        
        powerPlant.TurnOn();
        powerPlant.AdjustPower(110);

        Assert.Equal(maxCapacity, powerPlant.CurrentPower);
    }

    [Fact]
    void AdjustPower_WeatherFactor_ShouldReturnHalfPower()
    {
        string name = "Solar1";
        PowerPlantType type = PowerPlantType.Solar;
        decimal maxCapacity = 100;
        decimal minStableLoad = 0;
        decimal rampRate = 20;
        PowerPlant powerPlant = new PowerPlant(name, type, maxCapacity, minStableLoad, rampRate);
        
        powerPlant.TurnOn();
        powerPlant.AdjustPower(weatherFactor: 0.5);

        Assert.Equal(50, powerPlant.CurrentPower);
    }
}
