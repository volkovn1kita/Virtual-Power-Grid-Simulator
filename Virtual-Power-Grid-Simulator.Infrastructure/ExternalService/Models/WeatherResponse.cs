using System;

namespace Virtual_Power_Grid_Simulator.Infrastructure.ExternalService.Models;

public class WeatherResponse
{
    public MainData Main { get; set; }
    public WindData Wind { get; set; }
    public CloudsData Clouds { get; set; }
}

public class MainData
{
    public double Temp { get; set; }
}

public class WindData
{
    public double Speed { get; set; }
}

public class CloudsData
{
    public int All { get; set; }
}