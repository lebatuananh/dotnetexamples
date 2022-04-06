using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using GithubTrending.Domain.AggregateModel.GithubRepositoriesAggregate;
using GithubTrending.Infrastructure;
using GithubTrending.Infrastructure.Repositories;
using IdentityModel;
using IdentityModel.AspNetCore.AccessTokenValidation;
using IdentityServer4.AccessTokenValidation;

namespace GithubTrending.Api;

public static class Extensions
{
    public static readonly string
        AssemblyName = typeof(Extensions).GetTypeInfo().Assembly.GetName().Name ?? string.Empty;

    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        // TODO: MainDbContext
        services.AddDbContext<MainDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString(ConfigurationKeys.DefaultConnectionString), b =>
            {
                b.MigrationsAssembly(AssemblyName);
                b.MigrationsHistoryTable("__EFMigrationsHistory", MainDbContext.SchemaName);
                b.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            }).UseSnakeCaseNamingConvention();
            options.UseModel(MainDbContextModel.Instance);
        });

        // TODO: AuditLogDbContext
        services.AddDbContext<AuditLogDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString(ConfigurationKeys.DefaultConnectionString), b =>
            {
                b.MigrationsAssembly(AssemblyName);
                b.MigrationsHistoryTable("__EFMigrationsHistory", AuditLogDbContext.SchemaName);
                b.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            }).UseSnakeCaseNamingConvention();
            options.UseModel(AuditLogDbContextModel.Instance);
        });

        // TODO: LogDbContext
        services.AddDbContext<LogDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString(ConfigurationKeys.DefaultConnectionString), b =>
            {
                b.MigrationsAssembly(AssemblyName);
                b.MigrationsHistoryTable("__EFMigrationsHistory", LogDbContext.SchemaName);
                b.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            }).UseSnakeCaseNamingConvention();
            options.UseModel(LogDbContextModel.Instance);
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<MainDbContext>());
        services.AddScoped<IScopeContext, ScopeContext>();
        return services;
    }

    public static IServiceCollection AddRepository(this IServiceCollection services)
    {
        services.AddScoped<IGithubTrendingRepository, GithubTrendingRepository>();
        services.AddScoped<ILogRepository, LogRepository>();
        return services;
    }

    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services,
        IConfiguration configuration)
    {
        var adminApiConfiguration = configuration.GetSection("Authentication").Get<AuthenticationSettings>();

        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthorizationConsts.AdministrationPolicy,
                policy =>
                    policy.RequireAssertion(context => context.User.HasClaim(c =>
                            c.Type == $"http://schemas.microsoft.com/ws/2008/06/identity/claims/{JwtClaimTypes.Role}" &&
                            c.Value == adminApiConfiguration.AdministrationRole ||
                            c.Type == JwtClaimTypes.Role && c.Value == adminApiConfiguration.AdministrationRole ||
                            c.Type == $"client_{JwtClaimTypes.Role}" &&
                            c.Value == adminApiConfiguration.AdministrationRole
                        ) && context.User.HasClaim(c =>
                            c.Type == JwtClaimTypes.Scope && c.Value == adminApiConfiguration.ApiName)
                    ));
        });
        return services;
    }

    public static IServiceCollection AddAuthenticationCustom(this IServiceCollection services,
        IConfiguration configuration)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");
        var settings = configuration.GetSection("Authentication").Get<AuthenticationSettings>();
        services.AddAuthentication("token")

            // JWT tokens (default scheme)
            .AddJwtBearer("token", options =>
            {
                options.Authority = settings.Authority;
                options.Audience = settings.ApiName;
                // if token does not contain a dot, it is a reference token
                options.ForwardDefaultSelector = Selector.ForwardReferenceToken("introspection");
            })

            // reference tokens
            .AddOAuth2Introspection("introspection", options =>
            {
                options.Authority = settings.Authority;
                options.ClientId = settings.ApiName;
                options.ClientSecret = settings.ApiSecret;
            });
        return services;
    }


    public static IServiceCollection AddSwaggerConfig(this IServiceCollection services,
        IConfiguration configuration)
    {
        var settings = configuration.GetSection("Authentication").Get<AuthenticationSettings>();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Description = "Github Crawler web api implementation using Minimal Api in Asp.Net Core",
                Title = "Github Api",
                Version = "v1",
                Contact = new OpenApiContact
                {
                    Name = "MRA",
                    Url = new Uri("http://netcoreexamples.com")
                }
            });
            c.AddSecurityDefinition(IdentityServerAuthenticationDefaults.AuthenticationScheme,
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{settings.Authority}/connect/authorize"),
                            TokenUrl = new Uri($"{settings.Authority}/connect/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                { "iced_tea_api", "iced_tea_api" },
                                { "identity_admin_api", "identity_admin_api" },
                            }
                        }
                    }
                });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    new List<string> { "icedtea" }
                }
            });
        });
        return services;
    }
}