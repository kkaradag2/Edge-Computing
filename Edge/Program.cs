using Edge.Services;

var builder = WebApplication.CreateBuilder(args);

// Environment deðiþkenlerini ekle
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddControllers();

// Add Timer Background service
builder.Services.AddHostedService<TimedBackgroundService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();




app.Run();
