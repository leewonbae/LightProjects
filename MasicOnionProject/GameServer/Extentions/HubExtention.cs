using MessagePack;
using Snowpipe.Commons.BattleServerCommons;
using Snowpipe.Commons.BattleServerCommons.BattlePackets;

namespace BattleServer.Extentions
{
    public static class HubExtention
    {
        public static void SendToClient(this IBattleStreamingHubReceiver receiver, BaseScPacket packet)
        {
            receiver.OnReceivePacket(packet);
        }
        public static void SendToClient<T>(this IBattleStreamingHubReceiver receiver, T packet, DateTime serverDt) where T : IPacket
        {
            var newPacket = new BaseScPacket
            {
                ProtocolType = packet.ProtocolType,
                PacketBody = MessagePackSerializer.Serialize(packet),
                ServerDt = serverDt
            };

            receiver.OnReceivePacket(newPacket);
        }
    }
}
