using System.Text.Json;
using org.apache.zookeeper;

class Program
{
    static async Task Main(string[] args)
    {
        string zookeeperHost = "localhost:2181,localhost:2182,localhost:2183";
        string edgeNodesPath = "/services/edge_nodes";
        Random random = new Random();

        ZooKeeper zk = new ZooKeeper(zookeeperHost, 8000, new NodeWatcher());

        while (true)
        {
            List<string> healthyNodes = new List<string>();
            List<string> unhealthyNodes = new List<string>();

            try
            {
                // Alt düğümleri al
                var children = await zk.getChildrenAsync(edgeNodesPath, false);

                if (children != null && children.Children.Count > 0)
                {
                    foreach (var node in children.Children)
                    {
                        string nodePath = $"{edgeNodesPath}/{node}";
                        var dataResult = await zk.getDataAsync(nodePath);
                        if (dataResult?.Data != null)
                        {
                            var nodeDataJson = System.Text.Encoding.UTF8.GetString(dataResult.Data);
                            var nodeData = JsonSerializer.Deserialize<NodeData>(nodeDataJson);

                            if (nodeData != null)
                            {
                                if (nodeData.Status == "healthy" && (DateTime.Now - nodeData.UpdateAt).TotalSeconds <= 60)
                                {
                                    healthyNodes.Add($"{node} | URL: {nodeData.Url}");
                                }
                                else
                                {
                                    string reason = nodeData.Status != "healthy" ? "Unhealthy status" : "Outdated UpdateAt";
                                    unhealthyNodes.Add($"{node} | CPU: {nodeData.Cpu}, RAM: {nodeData.Ram} | Status: {nodeData.Status} | Reason: {reason}");
                                }
                            }
                        }
                        else
                        {
                            unhealthyNodes.Add($"{node} | Data: Invalid or null");
                        }
                    }

                    // Sağlıklı düğümlerden rastgele birine istek yap
                    if (healthyNodes.Count > 0)
                    {
                        int randomIndex = random.Next(healthyNodes.Count);
                        var selectedNode = healthyNodes[randomIndex];
                        string selectedNodeUrl = selectedNode.Split("| URL: ")[1].Trim();

                        try
                        {
                            using var httpClient = new HttpClient();
                            var response = await httpClient.GetAsync(selectedNodeUrl + "/WeatherForecast");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"[Request Sent] {selectedNode} | Response: {response.StatusCode} | Timestamp: {DateTime.Now}");
                            Console.ResetColor();
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"[Request Failed] {selectedNode} | Error: {ex.Message}");
                            Console.ResetColor();
                        }
                    }

                    // Sağlıksız düğümleri listele
                    Console.WriteLine("\n========== SUMMARY ==========");
                    Console.WriteLine($"Timestamp: {DateTime.Now}");

                    Console.WriteLine($"Healthy Nodes ({healthyNodes.Count}):");
                    Console.ForegroundColor = ConsoleColor.Green;
                    foreach (var healthyNode in healthyNodes)
                    {
                        Console.WriteLine($"  - {healthyNode}");
                    }
                    Console.ResetColor();

                    Console.WriteLine($"Unhealthy Nodes ({unhealthyNodes.Count}):");
                    Console.ForegroundColor = ConsoleColor.Red;
                    foreach (var unhealthyNode in unhealthyNodes)
                    {
                        Console.WriteLine($"  - {unhealthyNode}");
                    }
                    Console.ResetColor();

                    Console.WriteLine("=============================\n");
                }
                else
                {
                    Console.WriteLine("No nodes available under the specified path.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            await Task.Delay(2000); // 2 saniye bekleme
        }
    }

    public class NodeData
    {
        public string Url { get; set; }
        public float Cpu { get; set; }
        public float Ram { get; set; }
        public string Status { get; set; }
        public DateTime UpdateAt { get; set; }
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
