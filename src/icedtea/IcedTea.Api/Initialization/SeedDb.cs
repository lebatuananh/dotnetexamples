using IcedTea.Infrastructure;
using Shared.Audit;

namespace IcedTea.Api.Initialization;

public class SeedDb : IStage
{
    private readonly MainDbContext _dbContext;
    private readonly LogDbContext _logDbContext;
    private readonly AuditLogDbContext _auditLogDbContext;

    public SeedDb(MainDbContext dbContext, LogDbContext logDbContext, AuditLogDbContext auditLogDbContext)
    {
        _dbContext = dbContext;
        _logDbContext = logDbContext;
        _auditLogDbContext = auditLogDbContext;
    }

    public int Order => 1;

    public async Task ExecuteAsync()
    {
        await _dbContext.Database.MigrateAsync();
        await _logDbContext.Database.MigrateAsync();
        await _auditLogDbContext.Database.MigrateAsync();
    }
}