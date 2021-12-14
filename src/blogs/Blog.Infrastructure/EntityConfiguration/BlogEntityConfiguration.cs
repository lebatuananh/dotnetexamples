using Blog.Domain.AggregatesModel.BlogAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Constants;
using BlogEntity = Blog.Domain.AggregatesModel.BlogAggregate.Blog;


namespace Blog.Infrastructure.EntityConfiguration;

public class BlogEntityConfiguration : IEntityTypeConfiguration<BlogEntity>
{
    public void Configure(EntityTypeBuilder<BlogEntity> builder)
    {
        builder.ToTable("blog", BlogDbContext.SchemaName);
        builder.Property(x => x.Id).HasColumnType("uuid")
            .HasDefaultValueSql(PostgresDefaultAlgorithm.UuidAlgorithm);
        builder.Property(x => x.Description).HasMaxLength(256);
        builder.Property(x => x.Title).HasMaxLength(128).IsRequired();
        builder.Property(x => x.Status).HasDefaultValue(BlogStatus.Draft);
    }
}