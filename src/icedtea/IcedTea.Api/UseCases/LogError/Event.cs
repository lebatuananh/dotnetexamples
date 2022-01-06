using AuditLogging.Events;

namespace IcedTea.Api.UseCases.LogError;

public class ApiLogErrorAuditEventBase : AuditEvent
{
    protected ApiLogErrorAuditEventBase()
    {
        Category = "LogError";
    }
}