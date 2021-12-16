using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using Blog.Api.UseCases.Blog;
using Blog.Api.UseCases.Tags;
using Blog.Infrastructure;
using Blog.Infrastructure.Repositories;
using IdentityModel;
using IdentityModel.AspNetCore.AccessTokenValidation;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;

namespace Blog.Api;

public static class Extension
{
    public static readonly string
        AssemblyName = typeof(Extension).GetTypeInfo().Assembly.GetName().Name ?? string.Empty;

    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BlogDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString(ConfigurationKeys.DefaultConnectionString), b =>
            {
                b.MigrationsAssembly(AssemblyName);
                b.MigrationsHistoryTable("__EFMigrationsHistory", BlogDbContext.SchemaName);
                b.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            }).UseSnakeCaseNamingConvention();
            options.UseModel(BlogDbContextModel.Instance);
        });
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<BlogDbContext>());
        services.AddScoped<IScopeContext, ScopeContext>();
        return services;
    }

    public static IServiceCollection AddRepository(this IServiceCollection services)
    {
        services.AddScoped<IBlogRepository, BlogRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
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
                Description = "Blog web api implementation using Minimal Api in Asp.Net Core",
                Title = "Blog Api",
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
                                { "blog-api", "blog-api" }
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
                    new List<string> { "blog" }
                }
            });
        });
        return services;
    }

    public static WebApplication UseEndpoint(this WebApplication app)
    {
        #region BlogController

        app.MapGet("api/v1/blogs",
                async ([FromQuery] int skip, int take, string? query, ISender sender) =>
                    await sender.Send(new MutateBlog.GetListBlogQueries
                    {
                        Skip = skip,
                        Query = query,
                        Take = take
                    }))
            .Produces(200, typeof(ResultModel<QueryResult<BlogDto>>))
            .WithTags("Blogs")
            .ProducesProblem(404);

        app.MapPost("api/v1/blogs",
                [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
                async ([FromBody] MutateBlog.CreateBlogCommand command, ISender sender) =>
                    await sender.Send(command)).Produces(200)
            .WithTags("Blogs")
            .ProducesProblem(401);

        app.MapPut("api/v1/blogs/{id}",
                async ([FromRoute] Guid id, MutateBlog.UpdateBlogCommand command, ISender sender) =>
                    await sender.Send(command with { Id = id }))
            .Produces(200)
            .WithTags("Blogs");

        app.MapGet("api/v1/blogs/{id}",
                async ([FromRoute] Guid id, ISender sender) =>
                    await sender.Send(new MutateBlog.GetBlogQuery { Id = id }))
            .Produces(200, typeof(ResultModel<BlogDto>))
            .WithTags("Blogs").ProducesProblem(404);

        app.MapPut("api/v1/blogs/{id}/assign-tags",
                [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
                async ([FromRoute] Guid id, AssignBlogTag.AddTagCommand command, ISender sender) =>
                    await sender.Send(command with { BlogId = id })).Produces(200)
            .WithTags("Blogs")
            .ProducesProblem(401);

        app.MapPut("api/v1/blogs/{id}/remove-tags",
                [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
                async ([FromRoute] Guid id, AssignBlogTag.RemoveTagCommand command, ISender sender) =>
                    await sender.Send(command with { BlogId = id }))
            .Produces(200)
            .WithTags("Blogs")
            .ProducesProblem(401);

        #endregion

        #region TagController

        app.MapGet("api/v1/tags",
            async ([FromQuery] int skip, int take, string? query, ISender sender) =>
                await sender.Send(new MutateTag.GetListTagQueries
                {
                    Skip = skip,
                    Query = query,
                    Take = take
                })).Produces(200, typeof(ResultModel<QueryResult<TagDto>>)).WithTags("Tags").ProducesProblem(404);
        app.MapPost("api/v1/tags",
            [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
            async ([FromBody] MutateTag.CreateTagCommand command, ISender sender) =>
                await sender.Send(command)).WithTags("Tags").Produces(200).ProducesProblem(401);

        app.MapPut("api/v1/tags/{id}",
            [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
            async ([FromRoute] Guid id, MutateTag.UpdateTagCommand command, ISender sender) =>
                await sender.Send(command with { Id = id })).WithTags("Tags").Produces(200).ProducesProblem(401);

        app.MapGet("api/v1/tags/{id}",
                async ([FromRoute] Guid id, ISender sender) =>
                    await sender.Send(new MutateTag.GetTagQuery { Id = id })).Produces(200, typeof(ResultModel<TagDto>))
            .WithTags("Tags").ProducesProblem(404);

        #endregion

        return app;
    }
}