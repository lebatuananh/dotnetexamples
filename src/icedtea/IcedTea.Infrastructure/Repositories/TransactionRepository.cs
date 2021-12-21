using IcedTea.Domain.AggregateModel.TransactionAggregate;
using Shared.SeedWork;

namespace IcedTea.Infrastructure.Repositories;

public class TransactionRepository : Repository<Transaction, MainDbContext>, ITransactionRepository
{
    public TransactionRepository(MainDbContext dbContext) : base(dbContext)
    {
    }
}