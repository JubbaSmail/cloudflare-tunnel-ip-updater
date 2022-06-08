using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading;

namespace cloudflare_tunnel_ip_updater
{
	class MainClass
	{
		private static readonly HttpClient client = new HttpClient();
		static async Task Main(string[] args)
		{
			Console.WriteLine("Starting...");
			Console.WriteLine("Checking Env Variables...");
			string CLOUDFLARE_API_TOKEN = Environment.GetEnvironmentVariable("CLOUDFLARE_API_TOKEN");
			string CLOUDFLARE_TUNNEL_ID = Environment.GetEnvironmentVariable("CLOUDFLARE_TUNNEL_ID");
			string CLOUDFLARE_VIRTUAL_NET_ID = Environment.GetEnvironmentVariable("CLOUDFLARE_VIRTUAL_NET_ID");
			string CLOUDFLARE_TUNNEL_HOSTNAMES = Environment.GetEnvironmentVariable("CLOUDFLARE_TUNNEL_HOSTNAMES");

			Console.WriteLine("Checking the hostnames: " + CLOUDFLARE_TUNNEL_HOSTNAMES + " in the tunnel " + CLOUDFLARE_TUNNEL_ID);

			Thread.Sleep(60000);

		}

	}
}