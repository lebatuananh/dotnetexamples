using GithubTrending.Domain.AggregateModel.GithubRepositoriesAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Constants;

namespace GithubTrending.Infrastructure.EntityConfigurations;

public class GithubTrendingEntityConfiguration : IEntityTypeConfiguration<GithubRepositories>
{
    public void Configure(EntityTypeBuilder<GithubRepositories> builder)
    {
        builder.ToTable("github_trending", MainDbContext.SchemaName);
        builder
            .Property(x => x.Id)
            .HasColumnType("uuid")
            .HasDefaultValueSql(PostgresDefaultAlgorithm.UuidAlgorithm);
    }
}