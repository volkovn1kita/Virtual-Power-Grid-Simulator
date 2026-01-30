using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Virtual_Power_Grid_Simulator.Application.Interfaces;

namespace Virtual_Power_Grid_Simulator.Infrastructure.Workers;

public class SimulationWorker : BackgroundService
{
    private readonly IPowerGridService _powerGridService;
    private readonly ILogger<SimulationWorker> _logger;
    public SimulationWorker(
        IPowerGridService powerGridService,
        ILogger<SimulationWorker> logger)
    {
        _powerGridService = powerGridService;
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SimulationWorker started at: {time}", DateTimeOffset.Now);
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("SimulationWorker running at: {time}", DateTimeOffset.Now);
            _powerGridService.UpdateSimulationTime();

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
        _logger.LogInformation("SimulationWorker stopped at: {time}", DateTimeOffset.Now);
    }
}
