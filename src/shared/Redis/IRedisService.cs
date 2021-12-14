using System;
using System.Threading.Tasks;

namespace Shared.Redis;

public interface IRedisService
{
    Task<T> Get<T>(string key);
    Task<string> Get(string key);
    Task Set(string key, string value, TimeSpan expiration = default);
    Task Set<T>(string key, T obj, TimeSpan expiration = default);
    Task Remove(string key);
}