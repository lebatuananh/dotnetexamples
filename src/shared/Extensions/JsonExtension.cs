using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Shared.Extensions;

public static class JsonExtension
{
    public static T TryDeserialize<T>(this string str)
    {
        return string.IsNullOrEmpty(str) ? default : JsonConvert.DeserializeObject<T>(str);
    }

    public static T TryDeserialize<T>(this string str, JsonSerializerSettings settings)
    {
        return string.IsNullOrEmpty(str) ? default : JsonConvert.DeserializeObject<T>(str, settings);
    }

    public static object TryDeserialize(this string str, Type t, JsonSerializerSettings settings = null)
    {
        return string.IsNullOrEmpty(str) ? default : JsonConvert.DeserializeObject(str, t, settings);
    }

    public static Dictionary<string, string> DeserializeDictionaryIgnoreCase(this string str)
    {
        var result =
            new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        if (string.IsNullOrEmpty(str)) return result;

        JsonConvert.PopulateObject(str, result);
        return result;
    }
}