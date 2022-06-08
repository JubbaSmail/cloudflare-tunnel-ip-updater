using System.Text.Json.Serialization;

namespace cloudflare_tunnel_ip_updater
{
    public class CFTunnelRoute
    {
        [JsonPropertyName("network")]
        public string Network { get; set; }

        [JsonPropertyName("tunnel_id")]
        public string TunnelId { get; set; }

        [JsonPropertyName("tunnel_name")]
        public string TunnelName { get; set; }

        [JsonPropertyName("comment")]
        public string Comment { get; set; }

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }

        [JsonPropertyName("deleted_at")]
        public string DeletedAt { get; set; }

        [JsonPropertyName("virtual_network_id")]
        public string VirtualNetworkId { get; set; }
    }

    public class CFResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("messages")]
        public List<string> Messages { get; set; }

        [JsonPropertyName("errors")]
        public List<string> Errors { get; set; }

        [JsonPropertyName("result")]
        public List<CFTunnelRoute> Result { get; set; }
    }
}

