using BattleServer.Attributes;
using BattleServer.Exceptions;
using System.Reflection;

namespace BattleServer.Helpers
{
    public static class ServiceCollectionRegister
    {
        public static Dictionary<string, Type> _battleHandler { get; } = new Dictionary<string, Type>();
        public static void Register(IServiceCollection serviceCollections)
        {

            var injectableClassList = Assembly.GetExecutingAssembly().GetTypes()
                    .Where(t => t.GetCustomAttribute<BattleInjectableClassAttribute>() != null)
                    .ToList();

            foreach (var injectableClass in injectableClassList)
            {
                var lifeTime = injectableClass.GetCustomAttribute<BattleInjectableClassAttribute>()?.Lifetime;
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
            }
        }

        public static Type GetBattleHandler(string packetName)
        {
            if (_battleHandler.TryGetValue(packetName, out Type handlerWrapper))
            {
                return handlerWrapper;
            }

            throw new BattleServerException(Snowpipe.Commons.Packets.E_PACKET_ERROR_CODE.SERVER_ERROR, $"Packet handler for packet '{packetName}' not found.");
        }
    }
}
