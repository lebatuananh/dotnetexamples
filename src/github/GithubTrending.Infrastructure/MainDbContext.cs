using System.Reflection;
using GithubTrending.Domain.AggregateModel.GithubRepositoriesAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;
using Shared.Repositories;
using Shared.SeedWork;

namespace GithubTrending.Infrastructure;

public class MainDbContext : BaseDbContext
{
    public static string SchemaName => "github_repositories";
    public DbSet<GithubRepositories> GithubRepositories { get; set; }

    public MainDbContext(DbContextOptions<MainDbContext> options, IMediator mediator,
        IScopeContext scopeContext) : base(options, mediator, scopeContext)
    {
    }

    public MainDbContext(DbContextOptions<MainDbContext> options) : base(options)
    {
    }

    public MainDbContext()
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema(SchemaName);
        builder.HasPostgresExtension(PostgresDefaultAlgorithm.UuidGenerator);
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}