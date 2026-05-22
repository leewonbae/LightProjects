using BattleServer.Managers;
using MagicOnion.Server.Hubs;
using Snowpipe.Commons.BattleServerCommons;
using Snowpipe.Commons.BattleServerCommons.BattlePackets;

namespace BattleServer.Hubs
{
    public class BattleStreamingHub : StreamingHubBase<IBattleStreamingHub, IBattleStreamingHubReceiver>, IBattleStreamingHub
    {
        private readonly ILogger<BattleStreamingHub> _logger;
        private readonly PacketManager _packetManager;

        public BattleStreamingHub(ILogger<BattleStreamingHub> logger, PacketManager packetManager)
        {
            _logger = logger;
            _packetManager = packetManager;
        }

        protected override async ValueTask OnConnected()
        {
            _packetManager.AddReceiverQueue(Context.ContextId, Client);

            await base.OnConnected();
        }

        protected override async ValueTask OnDisconnected()
        {
            _packetManager.RemoveReciverQueue(Context.ContextId);

            await base.OnDisconnected();
        }

        //클라이언트로 부터 받은 패킷들
        public ValueTask SendToServerPacket(BaseCsPacket baseCsPacket)
        {
            _packetManager.EnqueuePacket(Context.ContextId, baseCsPacket);

            return ValueTask.CompletedTask;
        }
    }
}
