using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Initialization;

public static class InitializationExtenstion
{
    public static void AddInitializationStages(this IServiceCollection services)
    {
        AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly =>
                assembly.GetTypes()
                    .Where(t => typeof(IStage).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract))
            .ToList()
            .ForEach(stage => { services.AddTransient(typeof(IStage), stage); });
    }
}