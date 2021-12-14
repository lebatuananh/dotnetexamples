using System.Reflection;
using Blog.Api.UseCases.Blog;
using Blog.Domain.AggregatesModel.BlogAggregate;
using Blog.Infrastructure;
using Blog.Infrastructure.Repositories;

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

        return services;
    }

    public static IServiceCollection AddRepository(this IServiceCollection services)
    {
        services.AddScoped<IBlogRepository, BlogRepository>();
        return services;
    }

    public static WebApplication UseController(this WebApplication app)
    {
        app.MapGet("api/v1/blog",
            async ([FromQuery] MutateBlog.GetQueries queries, ISender sender) => await sender.Send(queries));
        return app;
    }
}