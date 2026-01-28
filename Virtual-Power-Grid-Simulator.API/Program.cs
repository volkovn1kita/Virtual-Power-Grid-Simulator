using Virtual_Power_Grid_Simulator.Application.Interfaces;
using Virtual_Power_Grid_Simulator.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();
builder.Services.AddSingleton<IPowerGridService, PowerGridService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.Run();