using Microsoft.Extensions.DependencyInjection;

namespace AuditLogging.Extensions
{
    public interface IAuditLoggingBuilder
    {
        IServiceCollection Services { get; }
    }
}