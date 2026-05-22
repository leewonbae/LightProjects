using Snowpipe.Commons.Packets;

namespace BattleServer.Exceptions
{
    public class BattleServerException : Exception
    {
        public E_PACKET_ERROR_CODE PacketErrorCode { get; set; }
        public BattleServerException(E_PACKET_ERROR_CODE packetErrorCode, string message) : base(message)
        {
            PacketErrorCode = packetErrorCode;
        }
    }
}
