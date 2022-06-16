# cloudflare-tunnel-ip-updater
Automatically update IPs of a private hostname to address this issue https://github.com/cloudflare/cloudflared/issues/644

# Why?
Currently, Cloudflare Tunnel supports adding a public hostname and private CIDR. However, we can't add a private hostname.
Engineers are using WARP to connect to internal company resources(ex: private K8S cluster in AWS); at the same time, we don't want to open the entire VPC range, but routing the private IP instead of the private hostname of that K8S cluster proven to be unstable since the private IP will keep rotating.

# Local Usage

```bash
git clone git@github.com:JubbaSmail/cloudflare_tunnel_ip_updater.git
cd cloudflare_tunnel_ip_updater/
docker build --platform=linux/amd64 -t cloudflare_tunnel_ip_updater:latest .

docker run \
-e CLOUDFLARE_TUNNEL_HOSTNAMES="x.example.com,y.example.com" \
-e CLOUDFLARE_API_TOKEN="*******" \
-e CLOUDFLARE_ACCOUNT_ID="*******" \
-e CLOUDFLARE_TUNNEL_ID="*******" \
-e CLOUDFLARE_VIRTUAL_NET_ID="*******" \
cloudflare_tunnel_ip_updater:latest
```

## Environment Variables
CLOUDFLARE_TUNNEL_HOSTNAMES
> List of the private hostnames (comma separated), ex:x.example.com,y.example.com
CLOUDFLARE_API_TOKEN
> Follow Cloudflare Documentation to create the token: https://developers.cloudflare.com/api/tokens/create/, with the permessions: Cloudflare Tunnel:Edit, Cloudflare Tunnel:Read
CLOUDFLARE_ACCOUNT_ID
> Follow Cloudflare Documentation to find the account ID https://developers.cloudflare.com/fundamentals/get-started/basic-tasks/find-account-and-zone-ids/
CLOUDFLARE_TUNNEL_ID
> Can be found by running the command: ```bash cloudflared tunnel route ip list``` more details can be found here: https://github.com/cloudflare/cloudflared
CLOUDFLARE_VIRTUAL_NET_ID
> Can be found by running the command: ```bash cloudflared tunnel route ip list``` more details can be found here: https://github.com/cloudflare/cloudflared

# License

[Apache-2.0](/LICENSE)