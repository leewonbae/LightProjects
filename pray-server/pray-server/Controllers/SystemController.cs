using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using pray_server.Exceptions;
using pray_server.Handlers;
using pray_server.Helpers;
using Serilog;
using Snowpipe.Commons.Packets;

namespace pray_server.Controllers
{
    [ApiController]
    [Route("system")]
    public class SystemController : ControllerBase
    {
        private readonly IHostEnvironment _hostEnvironment;
        private readonly ILogger<SystemController> _logger;

        public SystemController(IHostEnvironment hostEnvironment, ILogger<SystemController> logger)
        {
            _hostEnvironment = hostEnvironment;
            _logger = logger;
        }

        [HttpGet]
        [Route("get-server-datetime")]
        public DateTime GetServerDateTime()
        {
            _logger.LogInformation("ServerDateTime:" + ServerDateTime.Now);

            Response.Headers.Append("server-date", ServerDateTime.Now.ToString("u"));

            return ServerDateTime.Now;
        }

        [HttpPost]
        [Route("set-server-datetime")]
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
