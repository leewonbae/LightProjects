using Newtonsoft.Json;
using pray_server.Commons;

namespace Snowpipe.Commons.Packets
{
    public class ReqLogin : IPacket
    {
        [JsonProperty(Required = Required.Always)]
        public E_LOGIN_PLATFORM_TYPE LoginPlatformType { get; set; }
        public string PlatformToken { get; set; }
    }

    public class ResLogin : IPacket
    {
        public GameAccountVo GameAccountVo { get; set; }
    }
}