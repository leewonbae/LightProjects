namespace GameServer.Extensions
{
    public class InjectableClassAttribute : Attribute
    {
        public ServiceLifetime Lifetime { get; }
        public InjectableClassAttribute(ServiceLifetime lifetime)
        {
            Lifetime = lifetime;
        }
    }
}
