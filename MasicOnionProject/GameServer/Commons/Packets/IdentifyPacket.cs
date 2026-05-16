using Newtonsoft.Json;
namespace Snowpipe.Commons.Packets
{
    public class ReqIdentify : IPacket
    {
        [JsonProperty(Required = Required.Always)]
        public E_LOGIN_PLATFORM_TYPE LoginPlatformType { get; set; }
        public string PlatformToken { get; set; }
    }

    public class ResIdentify : IPacket
    {
        public bool ExistsAccount { get; set; }
    }
}