namespace BattleServer.Attributes
{
    public class BattleInjectableClassAttribute : Attribute
    {
        public ServiceLifetime Lifetime { get; }
        public BattleInjectableClassAttribute(ServiceLifetime lifetime)
        {
            Lifetime = lifetime;
        }
    }
}
