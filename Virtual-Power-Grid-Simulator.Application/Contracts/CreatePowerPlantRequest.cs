using Virtual_Power_Grid_Simulator.Domain.Enums;

namespace Virtual_Power_Grid_Simulator.Application.Contracts;

public record class CreatePowerPlantRequest
{
    public string Name { get; init; }
    public PowerPlantType Type { get; init; }
    public decimal MaxCapacity { get; init; }
    public decimal MinStableLoad { get; init; }
    public decimal RampRate { get; init; }
}
