using System.Threading.Tasks;
using AuditLogging.EntityFramework.DbContexts;
using AuditLogging.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;

namespace Shared.Audit;

public class AuditLogDbContext : DbContext, IAuditLoggingDbContext<AuditLog>
{
    public AuditLogDbContext(DbContextOptions<AuditLogDbContext> dbContextOptions)
        : base(dbContextOptions)
    {
    }

    public const string SchemaName = "auditlog";
    public DbSet<AuditLog> AuditLog { get; set; }

    public Task<int> SaveChangesAsync()
    {
        return base.SaveChangesAsync();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema(SchemaName);
        builder.HasPostgresExtension(PostgresDefaultAlgorithm.UuidGenerator);
        builder.Entity<AuditLog>(x =>
            x.ToTable("audit_log", SchemaName)
        );
        base.OnModelCreating(builder);
    }
}