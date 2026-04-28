namespace Snowpipe.Commons.Packets
{
    public sealed class BaseReqPacket
    {
        public string PacketBody { get; set; }
    }

    public sealed class BaseResPacket
    {
        public E_PACKET_ERROR_CODE ErrorCode { get; set; } = E_PACKET_ERROR_CODE.SUCCESS;

        public string PacketBody { get; set; }
    }

    public class IPacket
    {
        public E_PACKET_ERROR_CODE ContentsErrorCode { get; set; } = E_PACKET_ERROR_CODE.SUCCESS;
    }
}
