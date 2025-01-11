using EdgeMetricsAPI.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EdgeMetricsAPI.Controllers
{
  
        [ApiController]
        [Route("[controller]")]
        public class MetricsController : ControllerBase
        {
            private readonly IMetricsService _metricsService;

            public MetricsController(IMetricsService metricsService)
            {
                _metricsService = metricsService;
            }

            [HttpGet("/metrics")]
            public async Task<IActionResult> GetMetrics()
            {
                var metrics = await _metricsService.GetPrometheusMetrics();
                return Content(metrics, "text/plain");
            }
        }

    
}
