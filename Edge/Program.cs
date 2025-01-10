using Edge.Services;
using Hangfire;
using Hangfire.InMemory;
using HangfireBasicAuthenticationFilter;


var builder = WebApplication.CreateBuilder(args);


builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddControllers();

// Add Zookeeper Service
builder.Services.AddSingleton<IZookeeperService, ZookeeperService>();

// Hangfire'ý in-memory olarak yapýlandýr
// PostgreSQL veri deposu ile Hangfire'ý yapýlandýr
builder.Services.AddHangfire(configuration =>
    configuration.UseInMemoryStorage()
);

// Sunucu adý dahil arka plan iþlemcisi özelliklerini ayarlama
var containerName = builder.Configuration["AppSettings:CONTAINER_NAME"] ?? "default";
builder.Services.AddHangfireServer(options =>
{
    options.ServerName = $"HFS{containerName}";
});


builder.Services.AddHangfireServer();




var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    //AppPath = "" //The path for the Back To Site link. Set to null in order to hide the Back To  Site link.
    DashboardTitle = "Hangfire Login",
    Authorization = new[]
    {
        new HangfireCustomBasicAuthenticationFilter{
            User = "admin",
            Pass = "admin"
                }
            }
});

app.MapHangfireDashboard();


// 5 dakikada bir çalýþacak RecurringJob
RecurringJob.AddOrUpdate<IZookeeperService>(
    $"{containerName}_HealthCheck",
    zookeeper => zookeeper.HealthCheckSync(),
   "*/20 * * * * *"
);



app.Run();
