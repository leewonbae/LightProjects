using pray_server.Attributes;
using pray_server.Exceptions;
using System.Reflection;

namespace pray_server.Handlers
{
    public static class PacketHandlerRegister
    {
        public static Dictionary<string, Type> _registeredHandlerDic { get; } = new Dictionary<string, Type>();
        public static void Regist(IServiceCollection serviceCollection)
        {
            var packetHandlerList = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.GetCustomAttribute<PacketHandler>() != null)
                .ToList();

            foreach (var handler in packetHandlerList)
            {
                _registeredHandlerDic.Add(handler.Name, handler);

                serviceCollection.AddScoped(handler);
            }
        }

        public static Type GetHandler(string packetName)
        {
            if (_registeredHandlerDic.TryGetValue(packetName, out Type handler))
            {
                return handler;
            }

            throw new GameServerException(Snowpipe.Commons.Packets.E_PACKET_ERROR_CODE.SERVER_ERROR, $"Packet handler for packet '{packetName}' not found.");
        }
    }
}
