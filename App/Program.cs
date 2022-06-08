using System.Net;

namespace cloudflare_tunnel_ip_updater
{
	class MainClass
	{
		static async Task Main(string[] args)
		{
			/// 1. get IPs of the hostnames
			/// 2. get list of IPs in that tunnel and virtual net by comments=hostname
			///		a. if there is no result, get list of all IPs in that tunnel and virtual, check if the IP already exsit in the tunnel:
            ///			- if yes update and add comment
            ///			- if no, add the IP
			///		b. if IP has changed, delete old IPs and add new IP+comment
			///		c. if no changes, continue
			/// 4. sleep and exit

			Console.WriteLine("Starting...");
			Console.WriteLine("Checking Env Variables...");

            string CLOUDFLARE_ACCOUNT_ID = Environment.GetEnvironmentVariable("CLOUDFLARE_ACCOUNT_ID");
            string CLOUDFLARE_API_TOKEN = Environment.GetEnvironmentVariable("CLOUDFLARE_API_TOKEN");
            string CLOUDFLARE_TUNNEL_ID = Environment.GetEnvironmentVariable("CLOUDFLARE_TUNNEL_ID");
            string CLOUDFLARE_VIRTUAL_NET_ID = Environment.GetEnvironmentVariable("CLOUDFLARE_VIRTUAL_NET_ID");
            string CLOUDFLARE_TUNNEL_HOSTNAMES = Environment.GetEnvironmentVariable("CLOUDFLARE_TUNNEL_HOSTNAMES");

            if (CLOUDFLARE_ACCOUNT_ID == null ||
				CLOUDFLARE_API_TOKEN == null ||
				CLOUDFLARE_TUNNEL_ID == null ||
				CLOUDFLARE_VIRTUAL_NET_ID == null ||
				CLOUDFLARE_TUNNEL_HOSTNAMES == null)
			{
				Console.WriteLine("Error: missing one or more environment variables: CLOUDFLARE_ACCOUNT_ID, CLOUDFLARE_API_TOKEN, CLOUDFLARE_TUNNEL_ID, CLOUDFLARE_VIRTUAL_NET_ID, CLOUDFLARE_TUNNEL_HOSTNAMES");
				System.Environment.Exit(1);
			}
			Console.WriteLine("Checking the hostnames: " + CLOUDFLARE_TUNNEL_HOSTNAMES + " in the tunnel " + CLOUDFLARE_TUNNEL_ID);
			CloudflareAPI cf = new CloudflareAPI(CLOUDFLARE_ACCOUNT_ID, CLOUDFLARE_API_TOKEN, CLOUDFLARE_TUNNEL_ID, CLOUDFLARE_VIRTUAL_NET_ID);

			string[] hostnames = CLOUDFLARE_TUNNEL_HOSTNAMES.Split(',');

            foreach (var hostname in hostnames)
            {
				try
				{
					Console.WriteLine("Checking hostname: " + hostname);
					IPAddress[] HostIPAddress = Dns.GetHostAddresses(hostname);
					var TunnelIPs = await cf.GetTunnelIPsbyComment(hostname);
					if (TunnelIPs == null || TunnelIPs.Count == 0)
                    {
						TunnelIPs = await cf.GetAllTunnelIPs();
                        foreach (var hostip in HostIPAddress)
                        {
							bool found = false;
                            foreach (var tunnelip in TunnelIPs)
                            {
								if(hostip.ToString() == tunnelip.Network.Split('/')[0])
                                {
									found = true;
									if (await cf.AddCommenttoIP(hostip.ToString(), hostname))
									{
										Console.WriteLine("Comment: " + hostname + " added to the IP: " + hostip.ToString());
										break;
									}
								}
							}
							if(!found)
                            {
                                if(await cf.AddIPtoTunnel(hostip.ToString(), hostname))
									Console.WriteLine("IP: "+ hostip.ToString()+ " added with the comment: " + hostname );
							}
						}
					}
					else
                    {
						bool onefound = false;
						foreach (var tunnelip in TunnelIPs)
                        {
							bool found = false;
							foreach (var hostip in HostIPAddress)
							{
								if (hostip.ToString() == tunnelip.Network.Split('/')[0])
                                {
									found = true;
									onefound = true;
									break;
								}
							}
							if (!found)
							{
								if (await cf.DeleteIPFromTunnel(tunnelip.Network.Split('/')[0]))
									Console.WriteLine("IP: " + tunnelip.Network.Split('/')[0] + " deleted from hostname: " + hostname);
							}
						}
						if(!onefound)
                        {
							foreach (var hostip in HostIPAddress)
                            {
								if (await cf.AddIPtoTunnel(hostip.ToString(), hostname))
									Console.WriteLine("IP: " + hostip.ToString() + " added with the comment: " + hostname);
							}
						}
					}
				}
				catch
                {
					Console.WriteLine("Can't resolve the hostname: "+hostname);
					continue;
                }
				Console.WriteLine("-------------------------------");
			}
			Thread.Sleep(60000);
		}
	}
}