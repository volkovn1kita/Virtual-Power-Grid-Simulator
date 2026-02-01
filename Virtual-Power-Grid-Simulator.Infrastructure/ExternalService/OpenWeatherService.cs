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
    private WeatherInfo _cachedWeather;
    private DateTime _lastFetchTime = DateTime.MinValue;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(15);

    public OpenWeatherService(HttpClient httpClient, IConfiguration configuration, ILogger<OpenWeatherService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        
        _cachedWeather = new WeatherInfo(); 
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
            
            var latitude = _configuration["OpenWeatherMap:Lat"];
            var longitude = _configuration["OpenWeatherMap:Lon"];

            string url = $"https://api.openweathermap.org/data/3.0/onecall?lat={latitude}&lon={longitude}&exclude=minutely,hourly,daily,alerts&units=metric&appid={apiKey}";

            var data = await _httpClient.GetFromJsonAsync<OneCallResponse>(url);
;
            if (data?.Current != null)
            {
                _cachedWeather = new WeatherInfo
                {
                    TemperatureC = data.Current.Temp,
                    WindSpeed = data.Current.Wind_Speed,
                    CloudinessFactor = 1.0 - (data.Current.Clouds / 100.0 * 0.8) 
                };
                
                _lastFetchTime = DateTime.Now;
                _logger.LogInformation($"Weather updated from API: Temp={_cachedWeather.TemperatureC}, Wind={_cachedWeather.WindSpeed}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve weather data. Using cached data.");
        }

        return _cachedWeather;
    }
}
