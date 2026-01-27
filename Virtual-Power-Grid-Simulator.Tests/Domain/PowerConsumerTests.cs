using System;
using Virtual_Power_Grid_Simulator.Domain.Entities;
using Virtual_Power_Grid_Simulator.Domain.Enums;

namespace Virtual_Power_Grid_Simulator.Tests.Domain;

public class PowerConsumerTests
{

    [Fact]
    public void Constructor_ValidParameters_ShouldCreatePowerConsumer()
    {
        string name = "Residential complex 'Sofia Sphere'";
        ConsumerType type = ConsumerType.Residential;
        decimal maxPeakLoad = 100;
        int priority = 1;

        PowerConsumer powerConsumer = new PowerConsumer(name, type, maxPeakLoad, priority);
        
        Assert.Equal(name, powerConsumer.Name);   
        Assert.Equal(type, powerConsumer.Type);   
        Assert.Equal(maxPeakLoad, powerConsumer.MaxPeakLoad);   
        Assert.Equal(priority, powerConsumer.Priority);   
        Assert.True(powerConsumer.IsActive);   
    }

    [Fact]
    public void Constructor_ValidParameterMaxPeakLoad_ShouldThrowArgumentException()
    {
        string name = "Residential complex 'Sofia Sphere'";
        ConsumerType type = ConsumerType.Residential;
        decimal maxPeakLoad = -100;
        int priority = 1;

        Assert.Throws<ArgumentException>(() => {
            PowerConsumer powerConsumer = new PowerConsumer(name, type, maxPeakLoad, priority);
        });
    }

    [Fact]
    public void Constructor_ValidParameterPriority_ShouldThrowArgumentException()
    {
        string name = "Residential complex 'Sofia Sphere'";
        ConsumerType type = ConsumerType.Residential;
        decimal maxPeakLoad = 100;
        int priority = 0;

        Assert.Throws<ArgumentException>(() => {
            PowerConsumer powerConsumer = new PowerConsumer(name, type, maxPeakLoad, priority);
        });

        priority = 11;
        Assert.Throws<ArgumentException>(() => {
            PowerConsumer powerConsumer = new PowerConsumer(name, type, maxPeakLoad, priority);
        });
    }

    [Fact]
    void ConnectMethod_ShouldSetIsActiveToTrue()
    {
        //Arrange
        PowerConsumer powerConsumer = new PowerConsumer("Residential complex", ConsumerType.Residential, 100, 1);
        
        //Act
        powerConsumer.Disconnect();
        powerConsumer.Connect();

        //Assert
        Assert.True(powerConsumer.IsActive);
    }

    [Fact]
    void DisconnectMethod_ShouldSetIsActiveToFalse()
    {
        //Arrange
        PowerConsumer powerConsumer = new PowerConsumer("Residential complex", ConsumerType.Residential, 100, 1);

        //Act
        powerConsumer.Disconnect();

        //Assert
        Assert.False(powerConsumer.IsActive);
    }

    [Fact]
    void CalculateConsumption_DisconnectedConsumer_ShouldReturnZero()
    {
        //Arrange
        PowerConsumer powerConsumer = new PowerConsumer("Residential complex", ConsumerType.Residential, 100, 1);
        
        //Act
        powerConsumer.Disconnect();
        decimal result = powerConsumer.CalculateConsumption(new DateTime(2026, 1, 27, 8, 0, 0));

        //Assert
        Assert.Equal(0 , result);
    }


    [Theory]
    // Ніч (00:00 - 05:00) -> 0.2
    [InlineData(0, 0.2)] 
    [InlineData(3, 0.2)]
    [InlineData(5, 0.2)]
    // Ранок пік (06:00 - 09:00) -> 1.0
    [InlineData(6, 1.0)]
    [InlineData(8, 1.0)]
    [InlineData(9, 1.0)]
    // День (10:00 - 17:00) -> 0.5
    [InlineData(10, 0.5)]
    [InlineData(12, 0.5)]
    [InlineData(17, 0.5)]
    // Вечір пік (18:00 - 23:00) -> 1.0
    [InlineData(18, 1.0)]
    [InlineData(21, 1.0)]
    [InlineData(23, 1.0)]
    public void CalculateConsumption_Residential_ShouldReturnCorrectLoad(int hour, decimal expectedCoef)
    {
        // Arrange
        var consumer = new PowerConsumer("Test House", ConsumerType.Residential, 100, 1);
        var time = new DateTime(2026, 1, 1, hour, 0, 0);

        // Act
        var result = consumer.CalculateConsumption(time);

        // Assert
        Assert.Equal(100 * expectedCoef, result);
    }

    [Theory]
    // Робочі години (08:00 - 18:00) -> 1.0
    [InlineData(8, 1.0)]
    [InlineData(12, 1.0)]
    [InlineData(18, 1.0)]
    // Нічна зміна / Простій -> 0.1
    [InlineData(0, 0.1)]
    [InlineData(7, 0.1)]
    [InlineData(19, 0.1)]
    public void CalculateConsumption_Industrial_ShouldReturnCorrectLoad(int hour, decimal expectedCoef)
    {
        // Arrange
        var consumer = new PowerConsumer("Azovstal", ConsumerType.Industrial, 100, 1);
        var time = new DateTime(2026, 1, 1, hour, 0, 0);

        // Act
        var result = consumer.CalculateConsumption(time);

        // Assert
        Assert.Equal(100 * expectedCoef, result);
    }

}
