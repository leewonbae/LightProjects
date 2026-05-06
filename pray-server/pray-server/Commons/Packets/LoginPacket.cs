using GameServer.Commons;
using Newtonsoft.Json;

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
        public string SessionToken { get; set; }
        public GameAccountVo GameAccountVo { get; set; }
    }
}