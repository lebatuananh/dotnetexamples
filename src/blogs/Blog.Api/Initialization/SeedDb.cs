using Blog.Infrastructure;

namespace Blog.Api.Initialization;

public class SeedDb : IStage
{
    private readonly BlogDbContext _dbContext;

    public SeedDb(BlogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public int Order => 1;

    public async Task ExecuteAsync()
    {
        await _dbContext.Database.MigrateAsync();
    }
}