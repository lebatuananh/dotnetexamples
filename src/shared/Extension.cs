using System;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Shared.Logging;
using Shared.SeedWork;
using Shared.Validator;

namespace Shared;

public static class Extension
{
    public static IServiceCollection AddCustomMediatR(this IServiceCollection services, Type[] types = null,
        Action<IServiceCollection> doMoreActions = null)
    {
        services.AddHttpContextAccessor();

        services.AddMediatR(types)
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>))
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        doMoreActions?.Invoke(services);

        return services;
    }

    public static IServiceCollection AddCustomValidators(this IServiceCollection services, Type[] types)
    {
        return services.Scan(scan => scan
            .FromAssembliesOf(types)
            .AddClasses(c => c.AssignableTo(typeof(IValidator<>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());
    }

    public static IServiceCollection AddCustomCors(this IServiceCollection services, string corsName = "api",
        Action<CorsOptions> optionsAction = null)
    {
        services.AddCors(options =>
        {
            if (optionsAction == null)
            {
                options.AddPolicy(corsName,
                    policy =>
                    {
                        policy
                            .AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            }
            else
            {
                optionsAction.Invoke(options);
            }
        });

        return services;
    }

    public static IApplicationBuilder UseCustomCors(this IApplicationBuilder app, string corsName = "api")
    {
        return app.UseCors(corsName);
    }

    public static IServiceCollection AddRepository(this IServiceCollection services, Type repoType)
    {
        services.Scan(scan => scan
            .FromAssembliesOf(repoType)
            .AddClasses(classes =>
                classes.AssignableTo(repoType)).As(typeof(IRepository<>)).WithScopedLifetime()
        );

        return services;
    }
}