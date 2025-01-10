using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Edge.Services
{

    public class TimedBackgroundService : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TimedBackgroundService> _logger;

        public TimedBackgroundService(IConfiguration configuration, ILogger<TimedBackgroundService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                DoWork(); // İşinizi burada yapabilirsiniz
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private void DoWork()
        {
      
            var containerName = _configuration["MySettings:CONTAINER_NAME"];

            if (containerName != null)
            {
                Console.WriteLine($"Konteyner {containerName}, İş çalıştırıldı: {DateTime.Now}");
                _logger.LogInformation($"Konteyner {containerName}, İş çalıştırıldı: {DateTime.Now}");
            }
            else
            {
                Console.WriteLine($"İş çalıştırıldı: {DateTime.Now}");
                _logger.LogInformation($"İş çalıştırıldı: {DateTime.Now}");
            }
        }
    }
}
