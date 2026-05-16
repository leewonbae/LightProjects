using MagicOnion;
using MagicOnion.Server.Hubs;
using Snowpipe.Commons.ServicePackets;
using Snowpipe.Commons.StreamingHubs;

namespace GameServer.Hubs
{
    public class ChatHub : StreamingHubBase<IChattingSteamingHub, IChattingSteamingHubReceiver>, IChattingSteamingHub
    {
        private ILogger<ChatHub> _logger;
        public ChatHub(ILogger<ChatHub> logger) 
        {
            _logger = logger;
        }

        protected override ValueTask OnConnected()
        {
            _logger.LogInformation("client Connected");

            return base.OnConnected();
        }

        public Task MatchRequestAsync(CsMatchRequestPacket packet)
        {
            throw new NotImplementedException();
        }

        public Task RoomOutAsync(CsRoomOutPacket packet)
        {
            throw new NotImplementedException();
        }

        public Task SendChatMessageAsync(CsChatMessagePacket packet)
        {
            throw new NotImplementedException();
        }
    }
}
