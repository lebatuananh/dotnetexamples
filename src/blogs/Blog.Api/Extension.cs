using System.Reflection;
using Blog.Api.UseCases.Blog;
using Blog.Api.UseCases.Tags;
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

    public static WebApplication UseController(this WebApplication app)
    {
        #region BlogController

        app.MapGet("api/v1/blogs",
            async ([FromQuery] int page, int pageSize, string? queries, ISender sender) =>
                await sender.Send(new MutateBlog.GetListBlogQueries
                {
                    Page = page,
                    Query = queries,
                    PageSize = pageSize
                })).Produces(200, typeof(ResultModel<QueryResult<BlogDto>>)).WithTags("Blogs");
        app.MapPost("api/v1/blogs",
            async ([FromBody] MutateBlog.CreateBlogCommand command, ISender sender) =>
                await sender.Send(command)).Produces(200).WithTags("Blogs");

        app.MapPut("api/v1/blogs/{id}",
            async ([FromRoute] Guid id, MutateBlog.UpdateBlogCommand command, ISender sender) =>
                await sender.Send(command with { Id = id })).Produces(200).WithTags("Blogs");

        app.MapGet("api/v1/blogs/{id}",
                async ([FromRoute] Guid id, ISender sender) =>
                    await sender.Send(new MutateBlog.GetBlogQuery { Id = id }))
            .Produces(200, typeof(ResultModel<BlogDto>))
            .WithTags("Blogs");

        app.MapPut("api/v1/blogs/{id}/assign-tags",
            async ([FromRoute] Guid id, AssignBlogTag.AddTagCommand command, ISender sender) =>
                await sender.Send(command with { BlogId = id })).Produces(200).WithTags("Blogs");

        app.MapPut("api/v1/blogs/{id}/remove-tags",
            async ([FromRoute] Guid id, AssignBlogTag.RemoveTagCommand command, ISender sender) =>
                await sender.Send(command with { BlogId = id })).Produces(200).WithTags("Blogs");

        #endregion


        #region TagController

        app.MapGet("api/v1/tags",
            async ([FromQuery] int page, int pageSize, string? queries, ISender sender) =>
                await sender.Send(new MutateTag.GetListTagQueries
                {
                    Page = page,
                    Query = queries,
                    PageSize = pageSize
                })).Produces(200, typeof(ResultModel<QueryResult<TagDto>>)).WithTags("Tags");
        app.MapPost("api/v1/tags",
            async ([FromBody] MutateTag.CreateTagCommand command, ISender sender) =>
                await sender.Send(command)).WithTags("Tags").Produces(200);

        app.MapPut("api/v1/tags/{id}",
            async ([FromRoute] Guid id, MutateTag.UpdateTagCommand command, ISender sender) =>
                await sender.Send(command with { Id = id })).WithTags("Tags").Produces(200);

        app.MapGet("api/v1/tags/{id}",
                async ([FromRoute] Guid id, ISender sender) =>
                    await sender.Send(new MutateTag.GetTagQuery { Id = id })).Produces(200, typeof(ResultModel<TagDto>))
            .WithTags("Tags");

        #endregion

        return app;
    }
}