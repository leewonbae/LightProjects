using Jose;
using Newtonsoft.Json.Linq;
using pray_server.Exceptions;
using pray_server.Extensions;
using pray_server.Helpers;
using Snowpipe.Commons.Packets;
using System.Security.Cryptography;

namespace pray_server.Managers;

[InjectableClass(ServiceLifetime.Singleton)]
public class AppleLoginVerifier : ILoginVerifier
{
    private const string APPLE_TOKEN_ISSUER = "https://appleid.apple.com";
    private const string APPLE_JWT_URI = "https://appleid.apple.com/auth/keys";

    public string VerifyToken(string token)
    {
        var headers = JWT.Headers<JWTHeader>(token, JoseUtil.GetJwtSettings());
        JWK key = new JWK(headers.KeyID, GetFederatedAppleCerts(headers.KeyID));
        if (key.IsExpired)
        {
            var newKey = new JWK(key.KeyID, GetFederatedAppleCerts(key.KeyID));
            key = newKey;
        }

        var payLoad = JWT.Decode<JWTPayload>(token, key.PublicKey, JoseUtil.GetJwtSettings());
        if (!payLoad.Issuer.Equals(APPLE_TOKEN_ISSUER))
        {
            // jwt 발행인이 다름
            throw new GameServerException(E_PACKET_ERROR_CODE.PLATFORM_LOGIN_INVALID_ISSUER,
                $"AppleLoginVerifier Invalud Issuer[ {payLoad.Issuer}]");
        }

        if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() >= payLoad.ExpireAt)
        {
            //  토큰 만료
            throw new GameServerException(E_PACKET_ERROR_CODE.PLATFORM_LOGIN_EXPIRATION_TOKEN,
                $"AppleLoginVerifier ExpirationTimeSeconds[{payLoad.ExpireAt}]");
        }

        return payLoad.Subject;
    }

    private static RSACryptoServiceProvider GetFederatedAppleCerts(string kid)
    {
        var res = JObject.Parse(HttpRequestUtil.GetAsync(APPLE_JWT_URI).Result);
        var keys = (JArray)res["keys"];

        var key = (JObject)keys
            .Where((k) => (string)k["kty"] == "RSA" && (string)k["kid"] == kid)
            .FirstOrDefault();

        if (key == null)
        {
            throw new GameServerException(E_PACKET_ERROR_CODE.PLATFORM_LOGIN_APPLE_CERT_KEY_ERROR,
                $"GetFederatedAppleCerts() apple cert key is null kid[{kid}]");
        }

        var provider = new RSACryptoServiceProvider();
        provider.ImportParameters(new RSAParameters
        {
            Modulus = Base64Url.Decode((string)key["n"]),
            Exponent = Base64Url.Decode((string)key["e"]),
        });

        return provider;
    }
}