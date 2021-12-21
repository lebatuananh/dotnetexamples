using IcedTea.Domain.AggregateModel.CashFundTransactionAggregate;
using Shared.SeedWork;

namespace IcedTea.Infrastructure.Repositories;

public class CashFundTransactionRepository : Repository<CashFundTransaction, MainDbContext>,
    ICashFundTransactionRepository
{
    public CashFundTransactionRepository(MainDbContext dbContext) : base(dbContext)
    {
    } 
}