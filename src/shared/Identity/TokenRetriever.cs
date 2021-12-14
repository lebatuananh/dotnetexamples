using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Shared.Redis;
using HttpClientInstance = System.Net.Http.HttpClient;

namespace Shared.Identity;

public class TokenRetriever
{
    private const string CachePrefix = "access_token";
    private readonly IRedisService _cache;
    private readonly ClientCredentials _credentials;
    private readonly HttpClientInstance _httpClient;

    public TokenRetriever(IOptions<ClientCredentials> options, IRedisService cache, HttpClientInstance httpClient)
    {
        _credentials = options.Value;
        _cache = cache;
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Authorization =
            new BasicAuthenticationHeaderValue(_credentials.ClientId, _credentials.ClientSecret);
    }

    public async Task<string> RequestToken()
    {
        var cacheKey = $"{CachePrefix}:{_credentials.Scope}";

        var cacheToken = await _cache.Get(cacheKey);

        if (!string.IsNullOrEmpty(cacheToken)) return cacheToken;

        var uri = $"{_credentials.Authority}/token";
        var content = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "scope", _credentials.Scope }
            });
        var response = await _httpClient.PostAsync(uri, content);
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseContent, new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        });
        await _cache.Set(cacheKey, tokenResponse.AccessToken, TimeSpan.FromSeconds(tokenResponse.ExpiresIn / 2));
        return tokenResponse.AccessToken;
    }
}