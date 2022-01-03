using System;
using System.Threading.Tasks;
using AuditLogging.EntityFramework.Entities;
using Shared.Repositories;

namespace Shared.Audit.Repository;

public interface IAuditLogRepository<TAuditLog> where TAuditLog : AuditLog
{
    bool AutoSaveChanges { get; set; }

    Task<QueryResult<TAuditLog>> GetAsync(string @event, string source, string category, DateTime? created,
        string subjectIdentifier, string subjectName, int skip = 0, int take = 10);

    Task DeleteLogsOlderThanAsync(DateTime deleteOlderThan);
}