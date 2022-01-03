using AuditLogging.EntityFramework.DbContexts;
using AuditLogging.EntityFramework.Entities;
using AuditLogging.EntityFramework.Repositories;
using AuditLogging.EntityFramework.Services;
using AuditLogging.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Audit.Repository;

namespace Shared.Audit;

public static class Extension
{
    public static IServiceCollection AddAuditEventLogging<TAuditLoggingDbContext, TAuditLog>(
        this IServiceCollection services, IConfiguration configuration)
        where TAuditLog : AuditLog, new()
        where TAuditLoggingDbContext : IAuditLoggingDbContext<TAuditLog>
    {
        var auditLoggingConfiguration = configuration.GetSection(nameof(AuditLoggingConfiguration))
            .Get<AuditLoggingConfiguration>();
        services.AddSingleton(auditLoggingConfiguration);

        services.AddAuditLogging(options => { options.Source = auditLoggingConfiguration.Source; })
            .AddEventData<ApiAuditSubject, ApiAuditAction>()
            .AddAuditSinks<DatabaseAuditEventLoggerSink<TAuditLog>>();

        // TODO: Repository Save Change Database (use nuget package)
        services
            .AddTransient<IAuditLoggingRepository<TAuditLog>,
                AuditLoggingRepository<TAuditLoggingDbContext, TAuditLog>>();

        // TODO: Repository for user interface
        services.AddTransient<IAuditLogRepository<TAuditLog>, AuditLogRepository<TAuditLoggingDbContext, TAuditLog>>();

        return services;
    }
}