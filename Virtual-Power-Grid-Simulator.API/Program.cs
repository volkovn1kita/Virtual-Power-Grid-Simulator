using Microsoft.EntityFrameworkCore;
using Virtual_Power_Grid_Simulator.Application.Interfaces;
using Virtual_Power_Grid_Simulator.Infrastructure.Persistence;
using Virtual_Power_Grid_Simulator.Infrastructure.Repositories;
using Virtual_Power_Grid_Simulator.Infrastructure.Services;
using Virtual_Power_Grid_Simulator.Infrastructure.Workers;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<PowerPlantRepository>();
builder.Services.AddScoped<PowerConsumerRepository>();
builder.Services.AddScoped<IPowerGridService, PowerGridService>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString)
);

builder.Services.AddHostedService<SimulationWorker>();

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

app.MapControllers();
app.Run();