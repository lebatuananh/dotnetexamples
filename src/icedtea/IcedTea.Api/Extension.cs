using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using IcedTea.Api.UseCases.CashFund;
using IcedTea.Api.UseCases.Customer;
using IcedTea.Api.UseCases.LogError;
using IcedTea.Domain.AggregateModel.CashFundAggregate;
using IcedTea.Domain.AggregateModel.CashFundTransactionAggregate;
using IcedTea.Domain.AggregateModel.CustomerAggregate;
using IcedTea.Domain.AggregateModel.LogErrorAggregate;
using IcedTea.Domain.AggregateModel.TransactionAggregate;
using IcedTea.Infrastructure;
using IcedTea.Infrastructure.Repositories;
using IcedTead.Api;
using IdentityModel;
using IdentityModel.AspNetCore.AccessTokenValidation;
using IdentityServer4.AccessTokenValidation;
using Shared.Audit;
using User.Api;

namespace IcedTea.Api;

public static class Extension
{
    public static readonly string
        AssemblyName = typeof(Extension).GetTypeInfo().Assembly.GetName().Name ?? string.Empty;

    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
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

        services.AddDbContext<LogDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString(ConfigurationKeys.AdminLogDbConnection), b =>
            {
                b.MigrationsAssembly(AssemblyName);
                b.MigrationsHistoryTable("__EFMigrationsHistory");
                b.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            });
            options.UseModel(LogDbContextModel.Instance);
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<MainDbContext>());
        services.AddScoped<IScopeContext, ScopeContext>();
        return services;
    }

    public static IServiceCollection AddApiClient(this IServiceCollection services)
    {
        services.AddUserApiClient();
        return services;
    }

    public static IServiceCollection AddRepository(this IServiceCollection services)
    {
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ICashFundRepository, CashFundRepository>();
        services.AddScoped<ICashFundTransactionRepository, CashFundTransactionRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
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

    public static IServiceCollection AddConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(sp =>
            configuration.GetSection("UserApi").Get<UserEndpointConfig>());
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
                Description = "IcedTea web api implementation using Minimal Api in Asp.Net Core",
                Title = "IcedTea Api",
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
                                { "iced-tea-api", "iced-tea-api" },
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

    public static WebApplication UseEndpoint(this WebApplication app)
    {
        app.UseCustomerEndpoint();
        app.UseCashFundEndpoint();
        app.UseLogErrorEndpoint();
        return app;
    }
}