using IcedTea.Domain.AggregateModel.CashFundTransactionAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Constants;

namespace IcedTea.Infrastructure.EntityConfigurations;

public class CashFundTransactionEntityConfiguration : IEntityTypeConfiguration<CashFundTransaction>
{
    public void Configure(EntityTypeBuilder<CashFundTransaction> builder)
    {
        builder.ToTable("cash_fund_transactions", MainDbContext.SchemaName);

        builder.Property(x => x.Id).HasColumnType("uuid")
            .HasDefaultValueSql(PostgresDefaultAlgorithm.UuidAlgorithm);
        
        builder.Property(x => x.CustomerId).HasColumnType("uuid");

        builder.HasOne(t => t.CashFund)
            .WithMany(x => x.CashFundTransactions)
            .HasForeignKey(t => t.CashFundId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}