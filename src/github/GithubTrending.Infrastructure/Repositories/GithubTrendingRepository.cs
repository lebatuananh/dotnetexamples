using GithubTrending.Domain.AggregateModel.GithubRepositoriesAggregate;
using Shared.SeedWork;

namespace GithubTrending.Infrastructure.Repositories;

public class GithubTrendingRepository : Repository<GithubRepositories, MainDbContext>, IGithubTrendingRepository
{
    public GithubTrendingRepository(MainDbContext dbContext) : base(dbContext)
    {
    }
}