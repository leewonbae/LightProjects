using MagicOnion;
using Snowpipe.Commons.BattleServerCommons.BattlePackets;

namespace Snowpipe.Commons.BattleServerCommons
{
    public interface IBattleStreamingHub : IStreamingHub<IBattleStreamingHub, IBattleStreamingHubReceiver>
    {
        ValueTask SendToServerPacket(BaseCsPacket baseCsPacket);
    }

    public interface IBattleStreamingHubReceiver
    {
        void OnReceivePacket(BaseScPacket baseScPacket);
    }
}
