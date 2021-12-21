using IcedTea.Domain.AggregateModel.CashFundAggregate;
using Shared.SeedWork;

namespace IcedTea.Infrastructure.Repositories;

public class CashFundRepository : Repository<CashFund, MainDbContext>, ICashFundRepository
{
    public CashFundRepository(MainDbContext dbContext) : base(dbContext)
    {
    }
}