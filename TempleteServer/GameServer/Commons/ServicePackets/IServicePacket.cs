using MessagePack;
using Snowpipe.Commons.Packets;

namespace Snowpipe.Commons.ServicePackets
{
    public interface ICsPacket
    {

    }

    public interface IScPacket
    {
        E_PACKET_ERROR_CODE PacketErrorCode { get; }
    }

    [MessagePackObject]
    public class CsMatchRequestPacket : ICsPacket
    {
        [Key(0)]
        public long AccountId { get; set; }
    }

    [MessagePackObject]
    public class ScRoomInPacket : IScPacket
    {
        [Key(0)]
        public E_PACKET_ERROR_CODE PacketErrorCode { get; set; }
        [Key(1)]
        public int RoomId { get; set; }
        [Key(2)]
        public List<long> AccountIdList { get; set; }
    }

    [MessagePackObject]
    public class ScRoomOutPacket : IScPacket
    {
        [Key(0)]
        public E_PACKET_ERROR_CODE PacketErrorCode { get; set; }
        [Key(1)]
        public int RoomId { get; set; }
        [Key(2)]
        public long AccountId { get; set; }  // 누가 나갔는지
        [Key(3)]
        public string Reason { get; set; }  // "leave" | "disconnect" | "kick"
    }

    [MessagePackObject]
    public class CsRoomOutPacket : ICsPacket
    {
        [Key(0)] public int RoomId { get; set; }
        [Key(1)] public long AccountId { get; set; }
    }

    [MessagePackObject]
    public class CsChatMessagePacket : ICsPacket
    {
        [Key(0)] public int RoomId { get; set; }
        [Key(1)] public long AccountId { get; set; }
        [Key(2)] public string Message { get; set; }
        [Key(3)] public DateTime SentDt { get; set; }
    }


    [MessagePackObject]
    public class ScChatMessagePacket : IScPacket
    {
        [Key(0)]
        public E_PACKET_ERROR_CODE PacketErrorCode { get; set; }
        [Key(1)]
        public int RoomId { get; set; }
        [Key(2)]
        public long AccountId { get; set; }
        [Key(3)]
        public string Message { get; set; }
        [Key(4)]
        public DateTime SentDt { get; set; }
    }


}
