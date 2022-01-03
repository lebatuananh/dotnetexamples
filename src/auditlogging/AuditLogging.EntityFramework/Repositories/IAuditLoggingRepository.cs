using System.Threading.Tasks;
using AuditLogging.EntityFramework.Entities;
using AuditLogging.EntityFramework.Helpers.Common;

namespace AuditLogging.EntityFramework.Repositories
{
    public interface IAuditLoggingRepository<TAuditLog>
    where TAuditLog : AuditLog
    {
        Task SaveAsync(TAuditLog auditLog);
        
    }
}