using Newtonsoft.Json;
using Snowpipe.Commons;

namespace Snowpipe.Commons.Packets
{
    public class ReqVerifyReceipt : IPacket
    {
        [JsonProperty(Required = Required.Always)]
        public E_MARKET_TYPE MarketType { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string ProductId { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string PackageName { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string TransactionId { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Receipt { get; set; }

        [JsonProperty(Required = Required.Always)]
        public int ShopId { get; set; }
    }

    public class ResVerifyReceipt : IPacket
    {
        public bool IsValid { get; set; } = false;

        public string TransactionId { get; set; } = string.Empty;
    }
}
