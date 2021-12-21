using System.Reflection;
using IcedTea.Domain.AggregateModel.CashFundAggregate;
using IcedTea.Domain.AggregateModel.CustomerAggregate;
using IcedTea.Domain.AggregateModel.TransactionAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;
using Shared.Repositories;
using Shared.SeedWork;

namespace IcedTea.Infrastructure;

public class MainDbContext : BaseDbContext
{
    public static string SchemaName => "icedtea";
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<CashFund> CashFunds { get; set; }
    public DbSet<Wallet> Wallets { get; set; }

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