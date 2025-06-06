using GalavisorApi.Middleware;
using GalavisorApi.Services;
using GalavisorApi.Repositories;
using GalavisorApi.Data;
using GalavisorApi.Constants;
using GalavisorApi.Utils;

var builder = WebApplication.CreateBuilder(args);

var connectionString = ConfigStore.Get(ConfigKeys.DatabaseConnectionString);
builder.Services.AddSingleton(new DatabaseConnection(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddGoogleJwtAuthentication();
builder.Services.AddDefaultAuthorization();
builder.Services.AddHttpClient<AuthService>();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<ReviewService>();
builder.Services.AddSingleton<ReviewRepository>();
builder.Services.AddSingleton<UserRepository>();
builder.Services.AddSingleton<ActivityService>();
builder.Services.AddSingleton<PlanetService>();
builder.Services.AddSingleton<PlanetRepository>();
builder.Services.AddSingleton<TransportService>();

var app = builder.Build();
app.UseAuthorization();
app.MapControllers();

app.Run();