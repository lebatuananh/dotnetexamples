using IcedTea.Domain.AggregateModel.CustomerAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Constants;

namespace IcedTea.Infrastructure.EntityConfigurations;

public class CustomerEntityConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customer", MainDbContext.SchemaName);
        builder.Property(x => x.Id).HasColumnType("uuid")
            .HasDefaultValueSql(PostgresDefaultAlgorithm.UuidAlgorithm);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(128);
        builder.Property(x => x.UserName).IsRequired().HasMaxLength(128);
        builder.Property(x => x.DeviceId).HasMaxLength(20);
        builder
            .Property(x => x.ExternalId).IsRequired()
            .HasColumnType("uuid")
            .HasDefaultValueSql(PostgresDefaultAlgorithm.UuidAlgorithm);
        builder.Property(x => x.Status).HasDefaultValue(1);
        builder
            .HasOne(w => w.Wallet)
            .WithOne(c => c.Customer)
            .HasForeignKey<Customer>(x => x.WalletId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}