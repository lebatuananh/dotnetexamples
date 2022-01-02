using Microsoft.Extensions.DependencyInjection;

namespace User.Api;

public static class Extension
{
    public static IServiceCollection AddUserApiClient(this IServiceCollection services)
    {
        services.AddTransient<IUserApi, UserApi>();
        return services;
    }
}