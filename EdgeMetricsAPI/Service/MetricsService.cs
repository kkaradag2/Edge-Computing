using System.Text.Json;
using System.Text;
using org.apache.zookeeper;
using EdgeMetricsAPI.Models;

namespace EdgeMetricsAPI.Service
{
    public class MetricsService : IMetricsService
    {
        private readonly ZooKeeper _zooKeeper;
        private const string ZookeeperPath = "/services/edge_nodes";
        private readonly IConfiguration _configuration;
        private readonly ILogger<MetricsService> _logger;
        private const int SessionTimeout = 6000;

        public MetricsService(IConfiguration configuration, ILogger<MetricsService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            var connectionString = _configuration["AppSettings:ZOOKEEPER_HOST"];
            _zooKeeper = new ZooKeeper(connectionString, SessionTimeout, new NodeWatcher());
        }

        public async Task<string> GetPrometheusMetrics()
        {
            var builder = new StringBuilder();
            var nodes = await _zooKeeper.getChildrenAsync(ZookeeperPath, false);

            _logger.LogInformation($"Nodes: {nodes.Children.Count}");

            foreach (var node in nodes.Children)
            {
                var dataResult = await _zooKeeper.getDataAsync($"{ZookeeperPath}/{node}");
                if (dataResult.Data == null) continue;

              

                var nodeDataJson = Encoding.UTF8.GetString(dataResult.Data);
                var nodeData = JsonSerializer.Deserialize<NodeData>(nodeDataJson);

                _logger.LogInformation($"Node: {node}, CPU: {nodeData.Cpu}, RAM: {nodeData.Ram}, Status: {nodeData.Status}");

                if (nodeData != null)
                {
                    builder.AppendLine($"edge_cpu{{node=\"{node}\"}} {nodeData.Cpu}");
                    builder.AppendLine($"edge_ram{{node=\"{node}\"}} {nodeData.Ram}");
                    builder.AppendLine($"edge_status{{node=\"{node}\"}} {(nodeData.Status == "healthy" ? 1 : 0)}");
                }
            }

            return builder.ToString();
        }

        // Watcher sınıfı
        public class NodeWatcher : Watcher
        {
            public override async Task process(WatchedEvent @event)
            {
                Console.WriteLine($"Watcher triggered: EventType={@event.get_Type()}, Path={@event.getPath()}");
                switch (@event.get_Type())
                {
                    case Watcher.Event.EventType.NodeDataChanged:
                        Console.WriteLine("Node data has changed!");
                        break;
                    case Watcher.Event.EventType.NodeCreated:
                        Console.WriteLine("Node has been created!");
                        break;
                    case Watcher.Event.EventType.NodeDeleted:
                        Console.WriteLine("Node has been deleted!");
                        break;
                    default:
                        Console.WriteLine("Other event occurred.");
                        break;
                }
            }
        }

    }
}
