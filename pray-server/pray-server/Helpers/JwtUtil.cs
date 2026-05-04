using Jose;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace GameServer.Helpers;

class JWTHeader
{
    [JsonProperty("kid")]
    public string KeyID { get; set; }
}

class JWK
{
    private readonly long expireAt = 0;

    public JWK(string kid, RSACryptoServiceProvider publicKey)
    {
        KeyID = kid;
        PublicKey = publicKey;
        expireAt = ServerDateTime.Now.AddHours(10).Ticks;
    }

    public string KeyID { get; }
    public RSACryptoServiceProvider PublicKey { get; }
    public bool IsExpired
    {
        get
        {
            return ServerDateTime.Now.CompareTo(new DateTime(expireAt)) > 0;
        }
    }
}

class JWTPayload
{
    [JsonProperty("iss")]
    public string Issuer { get; set; }
    [JsonProperty("aud")]
    public string Audience { get; set; }
    [JsonProperty("exp")]
    public long ExpireAt { get; set; }
    [JsonProperty("sub")]
    public string Subject { get; set; }
}

public class JoseUtil
{
    public static JwtSettings GetJwtSettings()
    {
        return new JwtSettings { JsonMapper = new NewtonsoftMapper() };
    }

    public class NewtonsoftMapper : IJsonMapper
    {
        public T Parse<T>(string json)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
            };

            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        public string Serialize(object obj)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
            };

            return JsonConvert.SerializeObject(obj, Formatting.Indented, settings);
        }
    }
}