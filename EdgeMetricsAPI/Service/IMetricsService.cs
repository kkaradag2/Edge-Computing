namespace EdgeMetricsAPI.Service
{
    public interface IMetricsService
    {
       Task<string> GetPrometheusMetrics();        
    }
}
