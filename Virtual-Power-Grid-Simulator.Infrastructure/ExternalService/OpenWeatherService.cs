using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Virtual_Power_Grid_Simulator.Application.Interfaces;
using Virtual_Power_Grid_Simulator.Application.Models;
using Virtual_Power_Grid_Simulator.Infrastructure.ExternalService.Models;
using System.Net.Http.Json;

namespace Virtual_Power_Grid_Simulator.Infrastructure.ExternalService;

public class OpenWeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OpenWeatherService> _logger;
    private static WeatherInfo _cachedWeather = new WeatherInfo();
    private static DateTime _lastFetchTime = DateTime.MinValue;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(15);

    public OpenWeatherService(HttpClient httpClient, IConfiguration configuration, ILogger<OpenWeatherService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }
    public async Task<WeatherInfo> GetCurrentWeatherAsync()
    {
        if (DateTime.Now - _lastFetchTime < _cacheDuration)
        {
            return _cachedWeather;
        }

        try 
        {
            var apiKey = _configuration["OpenWeatherMap:ApiKey"];
            var lat = _configuration["OpenWeatherMap:Lat"];
            var lon = _configuration["OpenWeatherMap:Lon"];

            string url = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&units=metric&appid={apiKey}";

            var data = await _httpClient.GetFromJsonAsync<WeatherResponse>(url);

            if (data != null)
            {
                _cachedWeather = new WeatherInfo
                {
                    TemperatureC = data.Main.Temp,
                    WindSpeed = data.Wind.Speed,
                    CloudinessFactor = 1.0 - (data.Clouds.All / 100.0 * 0.8) 
                };
                
                _lastFetchTime = DateTime.Now;
                _logger.LogInformation($"Weather (v2.5) updated: Temp={_cachedWeather.TemperatureC}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve weather data (v2.5). Using cached data.");
        }

        return _cachedWeather;
    }
}
