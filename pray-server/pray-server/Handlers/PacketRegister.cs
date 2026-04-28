using pray_server.Attributes;
using pray_server.Exceptions;
using System.Reflection;

namespace pray_server.Handlers
{
    public static class PacketRegister
    {
        public static Dictionary<string, Type> _hadlerWrapperDic { get; } = new Dictionary<string, Type>();
        public static void Regist(IServiceCollection serviceCollection)
        {
            var packetHandlerList = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.GetCustomAttribute<PacketHandler>() != null)
                .ToList();

            foreach (var handler in packetHandlerList)
            {
                var handlerInterface = handler.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHandler<,>));

                var arguments = handlerInterface.GenericTypeArguments;

                var reqType = arguments[0];
                var resType = arguments[1];

                var key = handler.Name.Replace("Handler", "").ToLower();
                var wrapperType = typeof(HandlerWrapper<,>).MakeGenericType(reqType, resType);

                _hadlerWrapperDic[key] = wrapperType;

                serviceCollection.AddScoped(wrapperType);
                serviceCollection.AddScoped(handler);

                // ✅ 이 한 줄 추가
                var handlerInterfaceType = typeof(IHandler<,>).MakeGenericType(reqType, resType);
                serviceCollection.AddScoped(handlerInterfaceType, handler);
            }
        }

        public static Type GetHandlerWrapper(string packetName)
        {
            if (_hadlerWrapperDic.TryGetValue(packetName, out Type handlerWrapper))
            {
                return handlerWrapper;
            }

            throw new GameServerException(Snowpipe.Commons.Packets.E_PACKET_ERROR_CODE.SERVER_ERROR, $"Packet handler for packet '{packetName}' not found.");
        }
    }
}
