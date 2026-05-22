using MessagePack;
using Snowpipe.Commons.Packets;

namespace Snowpipe.Commons.BattleServerCommons.BattlePackets
{
    public class BattlePacketAttribute : Attribute
    {
        public E_PROTOCOL_TYPE ProtocolType { get; }
        public BattlePacketAttribute(E_PROTOCOL_TYPE protocolType)
        {
            ProtocolType = protocolType;
        }
    }

    public enum E_PROTOCOL_TYPE
    {
        CS_BATTLE_MATCH_REQUEST,
        SC_BATTLE_MATCH_REQUEST_RESULT,
    }


    public interface IPacket
    {
        E_PROTOCOL_TYPE ProtocolType { get; }
    }

    [MessagePackObject]
    public class BaseCsPacket
    {
        [Key(0)]
        public E_PROTOCOL_TYPE ProtocolType { get; set; }
        [Key(1)]
        public string BattleSessionToken { get; set; }
        [Key(2)]
        public ReadOnlyMemory<byte> PacketBody { get; set; }// gc 최소화 하기 위해, byte[] 대신 ReadOnlyMemory<byte> 사용
        [Key(3)]
        public DateTime CreateDt { get; set; }
    }

    [MessagePackObject]
    public class BaseScPacket
    {
        [Key(0)]
        public E_PROTOCOL_TYPE ProtocolType { get; set; }
        [Key(1)]
        public E_PACKET_ERROR_CODE ErrorCode { get; set; } = E_PACKET_ERROR_CODE.SUCCESS;
        [Key(2)]
        public ReadOnlyMemory<byte> PacketBody { get; set; } // gc 최소화 하기 위해, byte[] 대신 ReadOnlyMemory<byte> 사용
        [Key(3)]
        public DateTime ServerDt { get; set; }
    }

    [MessagePackObject]
    public class CsBattleMatchRequestPacket : IPacket
    {
        [IgnoreMember]
        public E_PROTOCOL_TYPE ProtocolType => E_PROTOCOL_TYPE.CS_BATTLE_MATCH_REQUEST;
        [Key(0)]
        public long AccountId { get; set; }
    }

    [MessagePackObject]
    public class ScBattleMatchRequestResultPacket : IPacket
    {
        [IgnoreMember]
        public E_PROTOCOL_TYPE ProtocolType => E_PROTOCOL_TYPE.SC_BATTLE_MATCH_REQUEST_RESULT;
        [Key(0)]
        public int QueueCount { get; set; }
    }
}
