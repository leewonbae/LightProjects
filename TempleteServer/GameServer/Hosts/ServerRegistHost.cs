
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
        public string Port { get; set; } = "10041";
        public string EnvironmentValue { get; set; }

        public Dictionary<string, string> GetParams()
        {
            var paramDict = new Dictionary<string, string>
                {
                    { "name", Name },
                    { "ip", Ip },
                    { "port",Port},
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

        private ServerInfo _serverInfo;

        private string _controlTower;
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


            var urlStr = _configuration.GetValue<string>("Kestrel:Endpoints:Http:Url");
            var uri = new Uri(urlStr);
            _serverInfo.Port = uri.Port.ToString();


            var controlTowerStr = _configuration.GetSection("ServerInfo");
            if (controlTowerStr != null)
            {
                _controlTower = controlTowerStr.GetSection("ControlTowerServer").Value;
            }

            _serverInfo.EnvironmentValue = _configuration.GetSection("ASPNETCORE_ENVIRONMENT") != null ? _configuration.GetSection("ASPNETCORE_ENVIRONMENT").Value : string.Empty;


            var registerUrl = $"http://{_controlTower}{REGISTER_API}";

            var result = await HttpRequestUtil.SendPostAsync(registerUrl,
              _serverInfo.GetParams(), ContentMimeType.APPLICATION_JSON);

            var resultNode = JsonObject.Parse(result);
            _controlTowerId = resultNode["id"].ToString();

            _logger.LogInformation($"Registered Server: [{_serverInfo.Name}]  [{_serverInfo.Ip}:{_serverInfo.Port}]  [{_serverInfo.EnvironmentValue}] ");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            var deregisterUrl = $"http://{_controlTower}{DEREGISTER_API}";

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
