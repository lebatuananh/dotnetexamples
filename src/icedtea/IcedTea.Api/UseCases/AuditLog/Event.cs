using AuditLogging.Events;

namespace IcedTea.Api.UseCases.AuditLog;

public class ApiAuditLogAuditEventBase : AuditEvent
{
    protected ApiAuditLogAuditEventBase()
    {
        Category = "AuditLog";
    }
}