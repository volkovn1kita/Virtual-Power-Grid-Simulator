using System;

namespace Virtual_Power_Grid_Simulator.Application.Models;

public class WeatherInfo
{
    public double TemperatureC { get; set; }
    public double WindSpeed { get; set; }
    public double CloudinessFactor { get; set; } 
    public WeatherInfo()
    {
        TemperatureC = 20;
        WindSpeed = 5;
        CloudinessFactor = 1.0;
    }
}
