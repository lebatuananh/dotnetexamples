using Blog.Domain.AggregatesModel.BlogAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog.Infrastructure.EntityConfiguration;

public class BlogTagEntityConfiguration : IEntityTypeConfiguration<BlogTag>
{
    public void Configure(EntityTypeBuilder<BlogTag> builder)
    {
        builder.ToTable("blog_tag", BlogDbContext.SchemaName);
        builder.HasKey(x => new { x.BlogId, x.TagId });
        builder.Property(x => x.TagName).IsRequired().HasMaxLength(30);
        builder
            .HasOne(x => x.Blog)
            .WithMany(t => t.BlogTags)
            .HasForeignKey(x => x.BlogId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.Tag)
            .WithMany()
            .HasForeignKey(x => x.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}