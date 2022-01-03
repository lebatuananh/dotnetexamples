using System;
using System.Linq;
using System.Threading.Tasks;
using AuditLogging.EntityFramework.DbContexts;
using AuditLogging.EntityFramework.Entities;
using AuditLogging.EntityFramework.Helpers;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;
using Shared.Repositories;

namespace Shared.Audit.Repository;

public class AuditLogRepository<TDbContext, TAuditLog> : IAuditLogRepository<TAuditLog>
    where TDbContext : IAuditLoggingDbContext<TAuditLog>
    where TAuditLog : AuditLog
{
    protected readonly TDbContext DbContext;

    public AuditLogRepository(TDbContext dbContext)
    {
        DbContext = dbContext;
    }


    public bool AutoSaveChanges { get; set; } = true;

    public async Task<QueryResult<TAuditLog>> GetAsync(string @event, string source, string category, DateTime? created,
        string subjectIdentifier,
        string subjectName, int skip = 0, int take = 10)
    {
        var auditLogs = await DbContext.AuditLog
            .WhereIf(!string.IsNullOrEmpty(subjectIdentifier),
                log => log.SubjectIdentifier.Contains(subjectIdentifier))
            .WhereIf(!string.IsNullOrEmpty(subjectName), log => log.SubjectName.Contains(subjectName))
            .WhereIf(!string.IsNullOrEmpty(@event), log => log.Event.Contains(@event))
            .WhereIf(!string.IsNullOrEmpty(source), log => log.Source.Contains(source))
            .WhereIf(!string.IsNullOrEmpty(category), log => log.Category.Contains(category))
            .WhereIf(created.HasValue, log => log.Created.Date == created.Value.Date).ToQueryResultAsync(skip, take);
        return auditLogs;
    }

    public async Task DeleteLogsOlderThanAsync(DateTime deleteOlderThan)
    {
        var logsToDelete = await DbContext.AuditLog.Where(x => x.Created.Date < deleteOlderThan.Date).ToListAsync();

        if (logsToDelete.Count == 0) return;

        DbContext.AuditLog.RemoveRange(logsToDelete);

        await AutoSaveChangesAsync();
    }

    protected virtual async Task<int> AutoSaveChangesAsync()
    {
        return AutoSaveChanges ? await DbContext.SaveChangesAsync() : (int)SavedStatus.WillBeSavedExplicitly;
    }

    public virtual async Task<int> SaveAllChangesAsync()
    {
        return await DbContext.SaveChangesAsync();
    }
}