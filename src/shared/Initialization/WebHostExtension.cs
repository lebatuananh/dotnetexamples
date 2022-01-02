using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace Shared.Initialization;

public static class WebHostExtension
{
    public static async Task AutoInit(this IHost webHost, ILogger logger)
    {
        var policy = CreatePolicy(3, logger, nameof(WebApplication));
        using var scope = webHost.Services.CreateScope();
        foreach (var stage in scope.ServiceProvider.GetServices<IStage>().OrderBy(t => t.Order))
        {
            await policy.ExecuteAsync(async () => { await stage.ExecuteAsync(); });
        }
    }

    public static AsyncRetryPolicy CreatePolicy(int retries, ILogger logger, string prefix)
    {
        return Policy.Handle<Exception>().WaitAndRetryAsync(
            retries,
            retry => TimeSpan.FromSeconds(15),
            (exception, timeSpan, retry, ctx) =>
            {
                logger.LogError(exception,
                    "[{Prefix}] Exception {ExceptionType} with message {Message} detected on attempt {Retry} of {Retries}",
                    prefix, exception.GetType().Name, exception.Message, retry, retries);
            }
        );
    }
}