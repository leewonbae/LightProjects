
using GameServer.Helpers;
using GameServer.Utils;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net;
using System.Text.Json.Nodes;

namespace pray_server.Hosts
{
    public class ServerInfo
    {
        public string Name { get; set; }
        public string Ip { get; set; }
        public string Port { get; set; } = "5113";
        public string ProjectName { get; set; }
        public string EnvironmentValue { get; set; }

        public Dictionary<string, string> GetParams()
        {
            var paramDict = new Dictionary<string, string>
                {
                    { "name", Name },
                    { "ip", Ip },
                    { "port",Port},
                    { "project_name", ProjectName },
                    { "environment_value", EnvironmentValue },
                };

            return paramDict;
        }
    }

    public class ServerRegistHost : IHostedService
    {
        private readonly ILogger<ServerRegistHost> _logger;
        private readonly IConfiguration _configuration;

        private const string REGISTER_API = "/api/register";
        private const string DEREGISTER_API = "/api/deregister";

        private const string CONTROL_TOWER_SERVER = "localhost:4000";//"192.168.0.20:8080";

        private ServerInfo _serverInfo;

        private string _controlTowerId;

        public ServerRegistHost(ILogger<ServerRegistHost> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _serverInfo = new ServerInfo()
            {
                Ip = NetworkUtil.IPAddress,
                Name = Dns.GetHostName(),
            };

            _serverInfo.ProjectName = _configuration.GetSection("PROJECT_NAME") != null ? _configuration.GetSection("PROJECT_NAME").Value : string.Empty;
            _serverInfo.EnvironmentValue = _configuration.GetSection("ASPNETCORE_ENVIRONMENT") != null ? _configuration.GetSection("ASPNETCORE_ENVIRONMENT").Value : string.Empty;

            var registerUrl = $"http://{CONTROL_TOWER_SERVER}{REGISTER_API}";

            var result = await HttpRequestUtil.SendPostAsync(registerUrl,
              _serverInfo.GetParams(), ContentMimeType.APPLICATION_JSON);

            var resultNode = JsonObject.Parse(result);
            _controlTowerId = resultNode["id"].ToString();

            _logger.LogInformation($"Registered Server: [{_serverInfo.Name}]  [{_serverInfo.Ip}:{_serverInfo.Port}]  [{_serverInfo.EnvironmentValue}]  [{_serverInfo.ProjectName}]");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            var deregisterUrl = $"http://{CONTROL_TOWER_SERVER}{DEREGISTER_API}";

            var paramDict = new Dictionary<string, string>()
            {
                {"id", _controlTowerId }
            };

            var result = await HttpRequestUtil.SendPostAsync(deregisterUrl,
                paramDict, ContentMimeType.APPLICATION_JSON);

            _logger.LogInformation($"Deregistered Server:{result}");
        }
    }
}
