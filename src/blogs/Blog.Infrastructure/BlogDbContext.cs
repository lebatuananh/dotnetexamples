using System.Reflection;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;
using Shared.Repositories;
using Shared.SeedWork;
using BlogEntity = Blog.Domain.AggregatesModel.BlogAggregate.Blog;
using TagEntity = Blog.Domain.AggregatesModel.TagAggregate.Tag;
using BlogTagEntity = Blog.Domain.AggregatesModel.BlogAggregate.BlogTag;

namespace Blog.Infrastructure;

public class BlogDbContext : BaseDbContext
{
    public BlogDbContext(DbContextOptions<BlogDbContext> options, IMediator mediator,
        IScopeContext scopeContext) : base(options, mediator, scopeContext)
    {
    }

    public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
    {
    }

    public BlogDbContext()
    {
    }

    public static string SchemaName => "blog";

    public virtual DbSet<BlogEntity> Blogs { get; set; }
    public virtual DbSet<TagEntity> Tags { get; set; }
    public virtual DbSet<BlogTagEntity> BlogTags { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema(SchemaName);
        builder.HasPostgresExtension(PostgresDefaultAlgorithm.UuidGenerator);
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}