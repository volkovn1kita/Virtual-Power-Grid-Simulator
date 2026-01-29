namespace Virtual_Power_Grid_Simulator.Application.Contracts;

public record class UpdatePowerRequest
{
    public decimal TargetPower { get; set; }
}
