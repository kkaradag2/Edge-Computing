namespace EdgeMetricsAPI.Models
{
    public class NodeData
    {
        public string Url { get; set; }
        public float Cpu { get; set; }
        public float Ram { get; set; }
        public string Status { get; set; }
        public DateTime UpdateAt { get; set; }
    }
}
