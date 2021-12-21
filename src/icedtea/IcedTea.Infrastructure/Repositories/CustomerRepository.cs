using IcedTea.Domain.AggregateModel.CustomerAggregate;
using Shared.SeedWork;

namespace IcedTea.Infrastructure.Repositories;

public class CustomerRepository : Repository<Customer, MainDbContext>, ICustomerRepository
{
    public CustomerRepository(MainDbContext dbContext) : base(dbContext)
    {
    }
}