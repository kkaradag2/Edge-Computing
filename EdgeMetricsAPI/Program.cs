using EdgeMetricsAPI.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddSingleton<IMetricsService, MetricsService>();

var app = builder.Build();

// Configure endpoints
app.UseRouting();
app.MapControllers();
app.Run();
