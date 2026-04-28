using Google.Apis.Auth;
using pray_server.Exceptions;
using pray_server.Extensions;
using Snowpipe.Commons.Packets;

namespace pray_server.Managers;
[InjectableClass(ServiceLifetime.Singleton)]
public class GoogleLoginVerifier : ILoginVerifier
{
    private readonly List<string> issuer = new List<string>() {
        "accounts.google.com",
        "https://accounts.google.com"
    };

    public string VerifyToken(string token)
    {
        var payLoad = GoogleJsonWebSignature.ValidateAsync(token).Result;
        if (!payLoad.EmailVerified)
        {
            throw new GameServerException(E_PACKET_ERROR_CODE.PLATFORM_LOGIN_GOOGLE_EMAIL_FAIL_VERIFIED,
                $"GoogleLoginVerifier emailVerified[ {payLoad.EmailVerified}]");
        }

        if (!issuer.Contains(payLoad.Issuer))
        {
            // jwt 발행인이 다름
            throw new GameServerException(E_PACKET_ERROR_CODE.PLATFORM_LOGIN_INVALID_ISSUER,
                $"GoogleLoginVerifier Invalud Issuer[ {payLoad.Issuer}]");
        }

        long expireTimeSeconds = payLoad.ExpirationTimeSeconds ?? 0;
        if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() >= expireTimeSeconds)
        {
            // 토큰 만료
            throw new GameServerException(E_PACKET_ERROR_CODE.PLATFORM_LOGIN_EXPIRATION_TOKEN,
                $"GoogleLoginVerifier ExpirationTimeSeconds[{payLoad.ExpirationTimeSeconds}]");
        }

        return payLoad.Subject;
    }
}