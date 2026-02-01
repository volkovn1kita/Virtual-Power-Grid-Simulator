using System;
using Virtual_Power_Grid_Simulator.Application.Models;

namespace Virtual_Power_Grid_Simulator.Application.Interfaces;

public interface IWeatherService
{
    Task<WeatherInfo> GetCurrentWeatherAsync();
}
