using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Virtual_Power_Grid_Simulator.Application.Interfaces;

namespace Virtual_Power_Grid_Simulator.Infrastructure.Workers;

public class SimulationWorker : BackgroundService
{
   private readonly IServiceProvider _serviceProvider; // <--- Беремо провайдер
    private readonly ILogger<SimulationWorker> _logger;

    public SimulationWorker(IServiceProvider serviceProvider, ILogger<SimulationWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var myService = scope.ServiceProvider.GetRequiredService<IPowerGridService>();
                
                myService.UpdateSimulationTime();
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}
