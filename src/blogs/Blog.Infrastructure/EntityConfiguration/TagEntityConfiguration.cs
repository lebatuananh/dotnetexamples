using Blog.Domain.AggregatesModel.TagAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Constants;

namespace Blog.Infrastructure.EntityConfiguration;

public class TagEntityConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("tag", BlogDbContext.SchemaName);
        builder.Property(x => x.Id).HasColumnType("uuid")
            .HasDefaultValueSql(PostgresDefaultAlgorithm.UuidAlgorithm);
        builder.Property(x => x.Name).HasMaxLength(30).IsRequired();
    }
}