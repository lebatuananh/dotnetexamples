using System;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Microsoft.AspNetCore.Http;

namespace Shared.Identity;

public class CustomTokenRetriever
{
    public static Func<HttpRequest, string> FromHeaderAndQueryString(string headerScheme = "Bearer",
        string queryScheme = "access_token")
    {
        return request =>
        {
            var token = TokenRetrieval.FromAuthorizationHeader(headerScheme)(request);
            return !string.IsNullOrEmpty(token) ? token : TokenRetrieval.FromQueryString(queryScheme)(request);
        };
    }
}