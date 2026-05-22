using BattleServer.Attributes;
using BattleServer.Exceptions;
using BattleServer.Extentions;
using GameServer.Helpers;
using MessagePack;
using Snowpipe.Commons.BattleServerCommons;
using Snowpipe.Commons.BattleServerCommons.BattlePackets;
using System.Collections.Concurrent;

namespace BattleServer.Managers
{
    public class BattleAccountInfo
    {
        public long AccountId { get; set; }
        public int Score { get; set; }
        public int AccumulateWinCount { get; set; }
        public int AccumulateLoseCount { get; set; }
    }

    [BattleInjectableClass(ServiceLifetime.Singleton)]
    public class BattleManager
    {
        private ConcurrentQueue<BattleAccountInfo> _battleMatchQueue = new ConcurrentQueue<BattleAccountInfo>();

        public BattleManager()
        {

        }

        public void BattleMatchRequest(IBattleStreamingHubReceiver receiver, BaseCsPacket baseCsPacket)
        {
            var req = MessagePackSerializer.Deserialize<CsBattleMatchRequestPacket>(baseCsPacket.PacketBody);

            var scPacket = new ScBattleMatchRequestResultPacket()
            {
                QueueCount = 1000,
            };

            receiver.SendToClient(scPacket, ServerDateTime.Now);
        }
    }
}
