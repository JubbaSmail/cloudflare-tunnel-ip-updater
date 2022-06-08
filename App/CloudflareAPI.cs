using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace cloudflare_tunnel_ip_updater
{
	public class CloudflareAPI
	{
		string CLOUDFLARE_ACCOUNT_ID = "";
		string CLOUDFLARE_API_TOKEN = "";
		string CLOUDFLARE_TUNNEL_ID = "";
		string CLOUDFLARE_VIRTUAL_NET_ID = "";
		string CLOUDFLARE_API_ENDPOINT = "https://api.cloudflare.com/client/v4/accounts/";
		private static readonly HttpClient client = new HttpClient();

		public CloudflareAPI(string _CLOUDFLARE_ACCOUNT_ID, string _CLOUDFLARE_API_TOKEN, string _CLOUDFLARE_TUNNEL_ID, string _CLOUDFLARE_VIRTUAL_NET_ID)
		{
			CLOUDFLARE_ACCOUNT_ID = _CLOUDFLARE_ACCOUNT_ID;
			CLOUDFLARE_API_TOKEN = _CLOUDFLARE_API_TOKEN;
			CLOUDFLARE_TUNNEL_ID = _CLOUDFLARE_TUNNEL_ID;
			CLOUDFLARE_VIRTUAL_NET_ID = _CLOUDFLARE_VIRTUAL_NET_ID;
			CLOUDFLARE_API_ENDPOINT = CLOUDFLARE_API_ENDPOINT + CLOUDFLARE_ACCOUNT_ID;
			client.DefaultRequestHeaders.Add("Authorization", "Bearer " + CLOUDFLARE_API_TOKEN);
			client.DefaultRequestHeaders.Accept.Add(
				new MediaTypeWithQualityHeaderValue("application/json"));
		}

		public async Task<List<CFTunnelRoute>> GetTunnelIPsbyComment(string Comment)
        {
			var streamTask = client.GetStreamAsync(CLOUDFLARE_API_ENDPOINT +
				"/teamnet/routes?tunnel_id=" + CLOUDFLARE_TUNNEL_ID +
				"&is_deleted=false&virtual_network_id=" + CLOUDFLARE_VIRTUAL_NET_ID +
                "&comment=" + Comment);
			var cf_response = await JsonSerializer.DeserializeAsync<CFResponse>(await streamTask);
			return cf_response.Result;
		}

		public async Task<List<CFTunnelRoute>> GetAllTunnelIPs()
		{
			var streamTask = client.GetStreamAsync(CLOUDFLARE_API_ENDPOINT +
				"/teamnet/routes?tunnel_id=" + CLOUDFLARE_TUNNEL_ID +
				"&is_deleted=false&virtual_network_id=" + CLOUDFLARE_VIRTUAL_NET_ID);
			var cf_response = await JsonSerializer.DeserializeAsync<CFResponse>(await streamTask);
			return cf_response.Result;
		}

		public async Task<bool> AddIPtoTunnel(string IP, string Comment)
        {
            var payload = "{\"tunnel_id\": \"" + CLOUDFLARE_TUNNEL_ID +
				"\",\"comment\": \"" + Comment +
				"\",\"virtual_network_id\": \"" + CLOUDFLARE_VIRTUAL_NET_ID + "\"}";
            HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(CLOUDFLARE_API_ENDPOINT +
				"/teamnet/routes/network/"+ IP, content);

			return response.IsSuccessStatusCode;
		}

		public async Task<bool> AddCommenttoIP(string IP, string Comment)
		{
			var payload = "{\"tunnel_id\": \"" + CLOUDFLARE_TUNNEL_ID +
				"\",\"comment\": \"" + Comment +
				"\",\"virtual_network_id\": \"" + CLOUDFLARE_VIRTUAL_NET_ID + "\"}";
			HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");

			var response = await client.PatchAsync(CLOUDFLARE_API_ENDPOINT +
				"/teamnet/routes/network/" + IP, content);

			return response.IsSuccessStatusCode;
		}

		public async Task<bool> DeleteIPFromTunnel(string IP)
		{
			var response = await client.DeleteAsync(CLOUDFLARE_API_ENDPOINT +
				"/teamnet/routes/network/" + IP);

			return response.IsSuccessStatusCode;
		}
	}
}

