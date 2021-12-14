using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Shared.Utils;

public static class JsonUtils
{
    public static string Serialize<T>(T obj)
    {
        return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });
    }

    public static T Deserialize<T>(string str)
    {
        return string.IsNullOrEmpty(str)
            ? default
            : JsonConvert.DeserializeObject<T>(str, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
    }

    public static object Deserialize(string str, Type targetType)
    {
        return string.IsNullOrEmpty(str)
            ? null
            : JsonConvert.DeserializeObject(str, targetType, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
    }
}