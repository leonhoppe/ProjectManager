using System.Dynamic;
using System.Text.Json;
using Microsoft.Extensions.Options;
using ProjectManager.Backend.Options;

namespace ProjectManager.Backend.Apis; 

public interface IProxyApi {
    public Task<(int, int)> AddLocation(string projectId, int port);
    public Task RemoveLocation(int proxyId, int certificateId);
}

public sealed class ProxyApi : IProxyApi {
    private readonly HttpClient _client;
    private readonly ProxyOptions _options;

    public ProxyApi(IOptions<ProxyOptions> options) {
        _client = new HttpClient();
        _options = options.Value;
    }

    private async Task Login() {
        var result = await _client.PostAsJsonAsync(_options.Url + "/api/tokens",
            new ProxyAuth(_options.Email, _options.Password), JsonSerializerOptions.Default);
        var response = await result.Content.ReadFromJsonAsync<TokenResponse>();
        _client.DefaultRequestHeaders.Clear();
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {response?.token}");
    }

    public async Task<(int, int)> AddLocation(string projectId, int port) {
        if (!_options.Enable) return (-1, -1);
        await Login();
        var result = await _client.PostAsJsonAsync(_options.Url + "/api/nginx/proxy-hosts",
            new CreateData($"{projectId}.{_options.Domain}", _options.Host, port, _options.Email));
        dynamic data = await result.Content.ReadFromJsonAsync<ExpandoObject>();
        if (data == null) return (-1, -1);
        int id = Convert.ToInt32($"{data.id}");
        int certificateId = Convert.ToInt32($"{data.certificate_id}");
        return (id, certificateId);
    }

    public async Task RemoveLocation(int proxyId, int certificateId) {
        if (proxyId == -1 || certificateId == -1) return;
        await Login();
        await _client.DeleteAsync(_options.Url + "/api/nginx/proxy-hosts/" + proxyId);
        await _client.DeleteAsync(_options.Url + "/api/nginx/certificates/" + certificateId);
    }

    private sealed record ProxyAuth(string identity, string secret);
    private sealed record TokenResponse(string token, string expires);
    private sealed class CreateData {
        public string access_list_id { get; set; } = "0";
        public string advanced_config { get; set; } = "    location / {\r\n        proxy_pass http://%docker_ip%;\r\n        proxy_hide_header X-Frame-Options;\r\n    }";
        public bool allow_websocket_upgrade { get; set; } = true;
        public bool block_exploits { get; set; } = true;
        public bool caching_enabled { get; set; } = true;
        public string certificate_id { get; set; } = "new";
        public string[] domain_names { get; set; }
        public string forward_host { get; set; }
        public int forward_port { get; set; }
        public string forward_scheme { get; set; } = "http";
        public bool hsts_enabled { get; set; } = false;
        public bool hsts_subdomains { get; set; } = false;
        public bool http2_support { get; set; } = true;
        public string[] locations { get; set; } = Array.Empty<string>();
        public bool ssl_forced { get; set; } = true;
        public SslMeta meta { get; set; }

        public CreateData(string domain, string ip, int port, string email) {
            domain_names = new[] { domain };
            forward_host = ip;
            forward_port = port;
            meta = new SslMeta {
                letsencrypt_email = email
            };
            advanced_config = advanced_config.Replace("%docker_ip%", $"{ip}:{port}");
        }
    }
    private sealed class SslMeta {
        public bool dns_challenge { get; set; } = false;
        public bool letsencrypt_agree { get; set; } = true;
        public string letsencrypt_email { get; set; }
    }
}