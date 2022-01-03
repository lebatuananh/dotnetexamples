using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AuditLogging.EntityFramework.DbContexts;
using AuditLogging.EntityFramework.Entities;
using AuditLogging.EntityFramework.Helpers;
using AuditLogging.EntityFramework.Helpers.Common;

namespace AuditLogging.EntityFramework.Repositories
{
    public class AuditLoggingRepository<TDbContext, TAuditLog> : IAuditLoggingRepository<TAuditLog>
        where TDbContext : IAuditLoggingDbContext<TAuditLog>
        where TAuditLog : AuditLog
    {
        protected TDbContext DbContext;

        public AuditLoggingRepository(TDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public virtual async Task SaveAsync(TAuditLog auditLog)
        {
            await DbContext.AuditLog.AddAsync(auditLog);
            await DbContext.SaveChangesAsync();
        }
    }
}