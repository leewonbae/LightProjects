using GameServer.Extensions;

namespace GameServer.Managers;

[InjectableClass(ServiceLifetime.Singleton)]
public class DefaultLoginVerifier : ILoginVerifier
{
    public string VerifyToken(string token)
    {
        return token;
    }
}