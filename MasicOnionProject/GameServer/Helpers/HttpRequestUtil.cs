
using System.Net.Http.Headers;

namespace GameServer.Helpers;
public class ContentMimeType
{
    public const string APPLICATION_JSON = "application/json";
}

public static class HttpRequestUtil
{
    private static readonly HttpClient _httpClient = new();
    public static async Task<string> SendPostAsync(string url,
        Dictionary<string/*key*/, string/*value*/> requestParams,
        string requestContentType = null)
    {
        var content = JsonContent.Create(requestParams);
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(url),
            Method = HttpMethod.Post,
            Content = content
        };

        request.Content.Headers.ContentType = new MediaTypeHeaderValue(requestContentType);

        var response = _httpClient.SendAsync(request);
        return await response.Result.Content.ReadAsStringAsync();
    }

    public static async Task<string> GetAsync(string uri)
    {
        var response = await _httpClient.GetAsync(uri);
        var rawResponseData = await response.Content.ReadAsStringAsync();

        return rawResponseData;
    }
}

