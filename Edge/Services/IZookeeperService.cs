using System.Threading.Tasks;

namespace Edge.Services
{
    public interface IZookeeperService
    {
        Task HealthCheckSync();
    }
}
