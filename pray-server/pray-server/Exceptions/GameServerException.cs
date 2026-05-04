using Snowpipe.Commons.Packets;

namespace GameServer.Exceptions
{
    public class GameServerException : Exception
    {
        public E_PACKET_ERROR_CODE ErrorCode { get; set; }
        public GameServerException(E_PACKET_ERROR_CODE errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
