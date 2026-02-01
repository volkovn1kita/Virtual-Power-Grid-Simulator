using System;

namespace Virtual_Power_Grid_Simulator.Infrastructure.ExternalService.Models;

public class CurrentWeather
{
    public double Temp { get; set; }
    public int Clouds { get; set; }
    public double Wind_Speed { get; set; }
}
