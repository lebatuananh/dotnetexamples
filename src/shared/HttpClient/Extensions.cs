using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.HttpClient;

public static class Extensions
{
    public static IServiceCollection AddHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient(configuration["ClientApi"]).ConfigurePrimaryHttpMessageHandler(() =>
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            return handler;
        });
        services.AddTransient<IBaseApiClient, BaseApiClient>();

        return services;
    }
}