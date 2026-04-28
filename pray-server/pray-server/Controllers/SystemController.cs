using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using pray_server.Exceptions;
using pray_server.Handlers;
using pray_server.Helpers;
using Snowpipe.Commons.Packets;

namespace pray_server.Controllers
{
    [ApiController]
    [Route("system")]
    public class SystemController : ControllerBase
    {
        private readonly IHostEnvironment _hostEnvironment;

        public SystemController(IHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        [HttpGet]
        [Route("get-server-datetime")]
        public DateTime GetServerDateTime()
        {
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

            Response.Headers.Append("server-date", ServerDateTime.Now.ToString("u"));

            return ServerDateTime.Now;
        }
    }
}
