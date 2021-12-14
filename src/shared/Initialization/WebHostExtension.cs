using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Shared.Initialization;

public static class WebHostExtension
{
    public static IHost AutoInit(this IHost webHost)
    {
        using (var scope = webHost.Services.CreateScope())
        {
            foreach (var stage in scope.ServiceProvider.GetServices<IStage>().OrderBy(t => t.Order))
                stage.ExecuteAsync().Wait();
        }

        return webHost;
    }
}