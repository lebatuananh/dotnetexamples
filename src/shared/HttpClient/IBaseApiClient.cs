using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shared.HttpClient;

public interface IBaseApiClient
{
    Task<List<T>> GetListAsync<T>(string url, string clientName, string baseApiUrl,
        bool requiredLogin = false);

    Task<T> GetAsync<T>(string url, string clientName, string baseApiUrl, bool requiredLogin = false);

    Task<TResponse> PostAsync<TRequest, TResponse>(string url, string clientName, string baseApiUrl,
        TRequest requestContent,
        bool requiredLogin = true);

    Task<TResponse> PutAsync<TRequest, TResponse>(string url, string clientName,
        string baseApiUrl,
        TRequest requestContent,
        bool requiredLogin = true);

    Task<TResponse> DeleteAsync<TRequest, TResponse>(string url, TRequest requestContent, string clientName,
        string baseApiUrl,
        bool requiredLogin = true);

    Task<TResponse> DeleteAsync<TResponse>(string url, string clientName,
        string baseApiUrl,
        bool requiredLogin = true);

    Task<TResponse> PostAsync<TResponse>(string url, string clientName, string baseApiUrl,
        bool requiredLogin = true);
}