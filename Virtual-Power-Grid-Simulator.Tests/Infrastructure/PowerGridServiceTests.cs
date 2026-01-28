using System;
using Virtual_Power_Grid_Simulator.Domain.Entities;
using Virtual_Power_Grid_Simulator.Domain.Enums;
using Virtual_Power_Grid_Simulator.Infrastructure.Services;

namespace Virtual_Power_Grid_Simulator.Tests.Infrastructure;

public class PowerGridServiceTests
{

    [Fact]
    void RegisterPowerPlant_ShouldAddToList()
    {
        PowerGridService powerGridService = new PowerGridService();
        var plant = new PowerPlant("KNPP", PowerPlantType.Nuclear , 100, 40, 10);

        powerGridService.RegisterPowerPlant(plant);
        var retrievedPlant = powerGridService.GetPowerPlantById(plant.Id);
        
        Assert.Equal(plant, retrievedPlant);
    }

    [Fact]
    void GetPowerPlantById_ExistingId_ShouldReturnPlant()
    {
        PowerGridService powerGridService = new PowerGridService();
        var plant = new PowerPlant("SolarOne", PowerPlantType.Solar, 50, 20, 5);
        powerGridService.RegisterPowerPlant(plant);

        var retrievedPlant = powerGridService.GetPowerPlantById(plant.Id);

        Assert.NotNull(powerGridService.GetPowerPlantById(plant.Id));
        Assert.Equal(plant, retrievedPlant);
    }

    [Fact]
    void GetPowerPlantById_NonExistingId_ShouldThrowKeyNotFoundException()
    {
        PowerGridService powerGridService = new PowerGridService();
        var nonExistingId = Guid.NewGuid();

        Assert.Throws<KeyNotFoundException>(() => powerGridService.GetPowerPlantById(nonExistingId));
    }

    [Fact]
    void CalculateGridState_ShouldReturnSnapshotWithCorrectData()
    {
        PowerGridService powerGridService = new PowerGridService();
        var plant = new PowerPlant("WindFarm", PowerPlantType.Nuclear, 110, 100, 10);
        plant.TurnOn();
        var consumer = new PowerConsumer("FactoryA", ConsumerType.Industrial, 50, 2);
        powerGridService.RegisterPowerPlant(plant);
        powerGridService.RegisterConsumer(consumer);

        DateTime simulationTime = new DateTime(2024, 1, 1, 12, 0, 0);
        var snapshot = powerGridService.CalculateGridState(simulationTime);

        Assert.Equal(simulationTime, snapshot.TimeStamp);
        Assert.Equal(100, snapshot.TotalGeneration);
        Assert.Equal(50, snapshot.TotalDemand);
    }
}
