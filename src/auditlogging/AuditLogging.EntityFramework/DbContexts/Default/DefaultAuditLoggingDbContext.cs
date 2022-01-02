using Microsoft.EntityFrameworkCore;
using AuditLogging.EntityFramework.Entities;

namespace AuditLogging.EntityFramework.DbContexts.Default
{
    public class DefaultAuditLoggingDbContext : AuditLoggingDbContext<AuditLog>
    {
        public DefaultAuditLoggingDbContext(DbContextOptions<DefaultAuditLoggingDbContext> dbContextOptions) 
            : base(dbContextOptions)
        {

        }
    }
}