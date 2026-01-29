using Virtual_Power_Grid_Simulator.Domain.Enums;

namespace Virtual_Power_Grid_Simulator.Application.Contracts;

public record class CreatePowerConsumerRequest
{
    public string Name { get; init; }
    public ConsumerType Type { get; init; }
    public decimal MaxPeakLoad { get; init; }
    public int Priority { get; init; }
}
