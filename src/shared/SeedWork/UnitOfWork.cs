using System.Threading;
using System.Threading.Tasks;
using Shared.Repositories;

namespace Shared.SeedWork;

public class UnitOfWork<TDbContext> : IUnitOfWork where TDbContext : BaseDbContext
{
    private readonly TDbContext _dbContext;

    public UnitOfWork(TDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}