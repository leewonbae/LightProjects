using MagicOnion;
using Snowpipe.Commons.ServicePackets;

namespace Snowpipe.Commons.StreamingHubs
{
    // 클라이언트가 서버에게 호출할 메서드 모음
    public interface IChattingSteamingHub : IStreamingHub<IChattingSteamingHub, IChattingSteamingHubReceiver>
    {
        Task MatchRequestAsync(CsMatchRequestPacket packet);
        Task SendChatMessageAsync(CsChatMessagePacket packet);
        Task RoomOutAsync(CsRoomOutPacket packet);

    }

    // 클라이언트가 메시지를 받는 입구가 되는 인터페이스
    public interface IChattingSteamingHubReceiver
    {
        void OnRoomIn(ScRoomInPacket packet);
        void OnRoomOut(ScRoomOutPacket packet);
        void OnChatMessage(ScChatMessagePacket packet);

    }
}
