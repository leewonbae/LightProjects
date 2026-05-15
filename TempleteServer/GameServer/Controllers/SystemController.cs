using GameServer.Exceptions;
using GameServer.Handlers;
using GameServer.Helpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
using Snowpipe.Commons.Packets;

namespace GameServer.Controllers
{
    [ApiController]
    [Route("system")]
    public class SystemController : ControllerBase
    {
        private readonly IHostEnvironment _hostEnvironment;
        private readonly ILogger<SystemController> _logger;
        private readonly IConfiguration _configuration;

        public SystemController(IHostEnvironment hostEnvironment, ILogger<SystemController> logger, IConfiguration configuration)
        {
            _hostEnvironment = hostEnvironment;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("get-server-date-time")]
        public DateTime GetServerDateTime()
        {
            var projectName = _configuration.GetValue<string>("PROJECT_NAME", string.Empty);

            _logger.LogInformation("ServerDateTime:" + ServerDateTime.Now);

            Response.Headers.Append("server-date", ServerDateTime.Now.ToString("u"));

            return ServerDateTime.Now;
        }

        [HttpPost]
        [Route("set-server-date-time")]
        public DateTime SetServerDateTime(string newDateTime)
        {
            if (_hostEnvironment.IsProduction())
            {
                throw new Exception("invalid Req");
            }

            ServerDateTime.SetServerDateTime(DateTime.Parse(newDateTime));

            _logger.LogInformation("Updated ServerDateTime:" + ServerDateTime.Now);

            Response.Headers.Append("server-date", ServerDateTime.Now.ToString("u"));

            return ServerDateTime.Now;
        }
    }
}
