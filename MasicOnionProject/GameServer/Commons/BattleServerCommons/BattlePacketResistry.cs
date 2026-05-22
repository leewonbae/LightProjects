using Snowpipe.Commons.BattleServerCommons.BattlePackets;
using System.Net.Sockets;
using System.Reflection;

namespace Snowpipe.Commons.BattleServerCommons
{
    public static class BattlePacketResistry
    {
        private static readonly Dictionary<E_PROTOCOL_TYPE, Type> _typeToProtocol = new Dictionary<E_PROTOCOL_TYPE, Type>();
        private static readonly Dictionary<Type, E_PROTOCOL_TYPE> _protocolToType = new Dictionary<Type, E_PROTOCOL_TYPE>();

        static BattlePacketResistry()
        {

        }

        public static void Init()
        {
            Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.GetCustomAttribute<BattlePacketAttribute>() != null)
                .ToList()
                .ForEach(t =>
                {
                    var attribute = t.GetCustomAttribute<BattlePacketAttribute>();
                    _typeToProtocol[attribute.ProtocolType] = t;
                    _protocolToType[t] = attribute.ProtocolType;
                });
        }

        public static Type GetPacketType(E_PROTOCOL_TYPE protocolType)
        {
            if (_typeToProtocol.TryGetValue(protocolType, out Type? type))
            {
                return type;
            }

            throw new Exception($"Packet type for protocol {protocolType} not found.");
        }

        public static E_PROTOCOL_TYPE GetProtocolType(Type packetType)
        {
            if (_protocolToType.TryGetValue(packetType, out E_PROTOCOL_TYPE protocolType))
            {
                return protocolType;
            }

            throw new Exception($"Protocol type for packet {packetType.Name} not found.");
        }


    }
}
