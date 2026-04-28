using pray_server.Extensions;

namespace pray_server.Managers;

[InjectableClass(ServiceLifetime.Singleton)]
public class DefaultLoginVerifier : ILoginVerifier
{
    public string VerifyToken(string token)
    {
        return token;
    }
}