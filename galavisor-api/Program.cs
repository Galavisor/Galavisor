using GalavisorApi.Middleware;
using GalavisorApi.Services;
using GalavisorApi.Repositories;
using GalavisorApi.Data;
using GalavisorApi.Constants;


var builder = WebApplication.CreateBuilder(args);

var connectionString = ConfigStore.Get(ConfigKeys.DatabaseConnectionString);
builder.Services.AddSingleton(new DatabaseConnection(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSingleton<ReviewService>();
builder.Services.AddSingleton<ReviewRepository>();

builder.Services.AddSingleton<PlanetService>();
builder.Services.AddSingleton<PlanetRepository>();

var app = builder.Build();

app.MapControllers();

app.Run();