using System;
using Virtual_Power_Grid_Simulator.Domain.Entities;

namespace Virtual_Power_Grid_Simulator.Application.Interfaces;

public interface IPowerGridService
{
    Guid RegisterPowerPlant(PowerPlant powerPlant);
    Guid RegisterConsumer(PowerConsumer consumer);

    GridSnapshot CalculateGridState(DateTime simulationTime);

    IEnumerable<PowerPlant> GetAllPowerPlants();
    IEnumerable<PowerConsumer> GetAllConsumers();

    PowerPlant GetPowerPlantById(Guid id);
    PowerConsumer GetConsumerById(Guid id);

}
