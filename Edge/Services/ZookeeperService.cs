using org.apache.zookeeper;
using System.Text;
using static org.apache.zookeeper.Watcher.Event;
using static org.apache.zookeeper.ZooDefs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Edge.Services
{
    public class ZookeeperService : IZookeeperService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ZookeeperService> _logger;
        private const int SessionTimeout = 6000;

        public ZookeeperService(IConfiguration configuration, ILogger<ZookeeperService> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task HealthCheckSync()
        {
            var connectionString = _configuration["AppSettings:ZOOKEEPER_HOST"];
            if (string.IsNullOrEmpty(connectionString))
            {
                _logger.LogError("ZooKeeper connection string is null or empty.");
                return;
            }

            var nodeName = _configuration["AppSettings:CONTAINER_NAME"];
            if (string.IsNullOrEmpty(nodeName))
            {
                _logger.LogError("Node name is not defined.");
                return;
            }

            var edgeURL = _configuration["AppSettings:EDGE_HOST"];
            if (string.IsNullOrEmpty(edgeURL))
            {
                _logger.LogError("Node URL is not defined.");
                return;
            }

            var nodePath = $"/services/edge_nodes/{nodeName}";

            // Rastgele değer üretici
            var random = new Random();

            // CPU ve RAM değerlerini rastgele oluştur
            var cpuUsage = Math.Round(random.NextDouble() * 100, 2); // 0 ile 100 arasında
            var memoryUsage = Math.Round(random.NextDouble() * 100, 2); // 0 ile 100 arasında

            // Threshold değerleri
            const double CPU_THRESHOLD = 80;
            const double MEMORY_THRESHOLD = 90;

            // Durumu belirle
            var status = (cpuUsage > CPU_THRESHOLD || memoryUsage > MEMORY_THRESHOLD) ? "unhealthy" : "healthy";


            var nodeData = new
            {
                Url = edgeURL,
                Cpu = cpuUsage,
                Ram = memoryUsage,
                Status = status,
                UpdateAt = DateTime.Now
            };

           


            ZooKeeper zk = null;

            try
            {
                zk = new ZooKeeper(connectionString, SessionTimeout, new NodeWatcher(_logger));
                await EnsurePathExistsAsync(zk, nodePath);

                var nodeDataSerialized = JsonSerializer.Serialize(nodeData);
                var nodeDataBytes = Encoding.UTF8.GetBytes(nodeDataSerialized);

                if (await zk.existsAsync(nodePath, false) == null)
                {
                    await zk.createAsync(nodePath, nodeDataBytes, Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT);
                    _logger.LogInformation($"Node '{nodePath}' created with data: {nodeDataSerialized}");
                }
                else
                {
                    await zk.setDataAsync(nodePath, nodeDataBytes, -1);
                    _logger.LogInformation($"Node '{nodePath}' updated with data: {nodeDataSerialized}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while writing edge information to ZooKeeper.");
            }
            finally
            {
                if (zk != null)
                {
                    await zk.closeAsync();
                    _logger.LogInformation("ZooKeeper connection closed.");
                }
            }
        }

        private async Task EnsurePathExistsAsync(ZooKeeper zk, string path)
        {
            var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var currentPath = string.Empty;

            foreach (var part in parts)
            {
                currentPath = $"{currentPath}/{part}";
                if (await zk.existsAsync(currentPath, false) == null)
                {
                    await zk.createAsync(currentPath, null, Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT);
                    _logger.LogInformation($"Path '{currentPath}' created.");
                }
            }
        }

        private class NodeWatcher : Watcher
        {
            private readonly ILogger<ZookeeperService> _logger;

            public NodeWatcher(ILogger<ZookeeperService> logger)
            {
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            }

            public override async Task process(WatchedEvent @event)
            {
                if (@event == null) return;

                _logger.LogInformation($"Watcher triggered: Type={@event.get_Type()}, Path={@event.getPath()}");

                switch (@event.get_Type())
                {
                    case EventType.NodeDataChanged:
                        _logger.LogInformation("Node data has changed.");
                        break;
                    case EventType.NodeCreated:
                        _logger.LogInformation("Node has been created.");
                        break;
                    case EventType.NodeDeleted:
                        _logger.LogInformation("Node has been deleted.");
                        break;
                    default:
                        _logger.LogInformation("Other event occurred.");
                        break;
                }

                await Task.CompletedTask; // To comply with async method signature
            }
        }
    }
}
