using Newtonsoft.Json;

namespace Snowpipe.Commons.Packets
{
    public class ReqRegister : IPacket
    {
        [JsonProperty(Required = Required.Always)]
        public E_LOGIN_PLATFORM_TYPE LoginPlatformType { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string PlatformToken { get; set; } = "";
        [JsonProperty(Required = Required.Always)]
        public string Nickname { get; set; } = "";
    }

    public class ResRegister : IPacket
    {
    }
}