using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Shared.HttpClient;

public class BaseApiClient : IBaseApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BaseApiClient(IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<T>> GetListAsync<T>(string url, string clientName, string baseApiUrl,
        bool requiredLogin = false)
    {
        var client = _httpClientFactory.CreateClient(clientName);
        client.BaseAddress = new Uri(baseApiUrl);
        if (requiredLogin)
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        var response = await client.GetAsync(url);
        await PreprocessResponse<object>(response, null);
        var body = await response.Content.ReadAsStringAsync();
        var data = (List<T>)JsonConvert.DeserializeObject(body, typeof(List<T>));
        return data;
    }

    public async Task<T> GetAsync<T>(string url, string clientName, string baseApiUrl, bool requiredLogin = false)
    {
        var client = _httpClientFactory.CreateClient(clientName);
        client.BaseAddress = new Uri(baseApiUrl);
        if (requiredLogin)
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        var response = await client.GetAsync(url);
        await PreprocessResponse<object>(response, null);
        var body = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<T>(body);
        return data;
    }

    public async Task<TResponse> PostAsync<TResponse>(string url, string clientName, string baseApiUrl,
        bool requiredLogin = true)
    {
        var client = _httpClientFactory.CreateClient(clientName);
        client.BaseAddress = new Uri(baseApiUrl);
        if (requiredLogin)
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        var response = await client.PostAsync(url, null!);
        await PreprocessResponse<object>(response, null);
        var body = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode) return JsonConvert.DeserializeObject<TResponse>(body);

        throw new Exception(body);
    }

    public async Task<TResponse> PostAsync<TRequest, TResponse>(string url, string clientName, string baseApiUrl,
        TRequest requestContent,
        bool requiredLogin = true)
    {
        var client = _httpClientFactory.CreateClient(clientName);
        client.BaseAddress = new Uri(baseApiUrl);
        StringContent httpContent = null;
        if (requestContent != null)
        {
            var json = JsonConvert.SerializeObject(requestContent);
            httpContent = new StringContent(json, Encoding.UTF8, "application/json");
        }

        if (requiredLogin)
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        var response = await client.PostAsync(url, httpContent);
        await PreprocessResponse(response, requestContent);

        var body = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode) return JsonConvert.DeserializeObject<TResponse>(body);

        throw new Exception(body);
    }

    public async Task<TResponse> PutAsync<TRequest, TResponse>(string url,
        string clientName,
        string baseApiUrl,
        TRequest requestContent,
        bool requiredLogin = true)
    {
        var client = _httpClientFactory.CreateClient(clientName);
        client.BaseAddress = new Uri(baseApiUrl);
        HttpContent httpContent = null;
        if (requestContent != null)
        {
            var json = JsonConvert.SerializeObject(requestContent);
            httpContent = new StringContent(json, Encoding.UTF8, "application/json");
        }

        if (requiredLogin)
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        var response = await client.PutAsync(url, httpContent);
        await PreprocessResponse(response, requestContent);

        var body = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
            return JsonConvert.DeserializeObject<TResponse>(body);

        throw new Exception(body);
    }

    public async Task<TResponse> DeleteAsync<TRequest, TResponse>(string url, TRequest requestContent,
        string clientName,
        string baseApiUrl,
        bool requiredLogin = true)
    {
        var client = _httpClientFactory.CreateClient(clientName);
        client.BaseAddress = new Uri(baseApiUrl);
        StringContent httpContent = null;
        if (requestContent != null)
        {
            var json = JsonConvert.SerializeObject(requestContent);
            httpContent = new StringContent(json, Encoding.UTF8, "application/json");
        }

        if (requiredLogin)
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        var request = new HttpRequestMessage
        {
            Content = httpContent,
            Method = HttpMethod.Delete,
            RequestUri = new Uri($"{baseApiUrl}{url}")
        };
        var response = await client.SendAsync(request);
        await PreprocessResponse(response, requestContent);

        var body = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode) return JsonConvert.DeserializeObject<TResponse>(body);

        throw new Exception(body);
    }

    public async Task<TResponse> DeleteAsync<TResponse>(string url, string clientName, string baseApiUrl,
        bool requiredLogin = true)
    {
        var client = _httpClientFactory.CreateClient(clientName);
        client.BaseAddress = new Uri(baseApiUrl);
        if (requiredLogin)
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        var response = await client.DeleteAsync(url);
        await PreprocessResponse<object>(response, null);
        var body = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode) return JsonConvert.DeserializeObject<TResponse>(body);

        throw new Exception(body);
    }

    public virtual async Task PreprocessResponse<TRequest>(HttpResponseMessage responseMessage, TRequest request)
    {
        if (!responseMessage.IsSuccessStatusCode)
        {
            var bodyText = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new Exception(bodyText);
        }
    }
}