using pray_server.Exceptions;
using pray_server.Extensions;
using pray_server.Handlers;
using System.Reflection;

namespace pray_server.Helpers
{
    public static class ServiceCollectionRegister
    {
        public static Dictionary<string, Type> _hadlerWrapperDic { get; } = new Dictionary<string, Type>();
        public static void Register(IServiceCollection serviceCollections)
        {

            var injectableClassList = Assembly.GetExecutingAssembly().GetTypes()
                    .Where(t => t.GetCustomAttribute<InjectableClassAttribute>() != null)
                    .ToList();

            foreach (var injectableClass in injectableClassList)
            {
                var lifeTime = injectableClass.GetCustomAttribute<InjectableClassAttribute>()?.Lifetime;
                switch (lifeTime)
                {
                    case ServiceLifetime.Singleton:
                        serviceCollections.AddSingleton(injectableClass);
                        break;
                    case ServiceLifetime.Scoped:
                        serviceCollections.AddScoped(injectableClass);
                        break;
                    case ServiceLifetime.Transient:
                        serviceCollections.AddTransient(injectableClass);
                        break;
                }

                if (injectableClass.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHandler<,>)))
                {
                    // If the class implements IHandler<,>, register it as a handler
                    HandlerRegister(serviceCollections, injectableClass);
                }
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

        private static void HandlerRegister(IServiceCollection serviceCollections, Type handlerClass)
        {
            var handlerInterface = handlerClass.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHandler<,>));

            var arguments = handlerInterface.GenericTypeArguments;

            var reqType = arguments[0];
            var resType = arguments[1];

            var key = handlerClass.Name.Replace("Handler", "").ToLower();
            var wrapperType = typeof(HandlerWrapper<,>).MakeGenericType(reqType, resType);

            _hadlerWrapperDic[key] = wrapperType;

            serviceCollections.AddScoped(wrapperType);

            // ✅ 이 한 줄 추가
            var handlerInterfaceType = typeof(IHandler<,>).MakeGenericType(reqType, resType);
            serviceCollections.AddScoped(handlerInterfaceType, handlerClass);
        }
    }
}
