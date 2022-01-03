using AuditLogging.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace Shared.Audit;

public class ApiAuditAction : IAuditAction
{
    public ApiAuditAction(IHttpContextAccessor accessor)
    {
        Action = new
        {
            accessor.HttpContext!.TraceIdentifier,
            RequestUrl = accessor.HttpContext.Request.GetDisplayUrl(),
            HttpMethod = accessor.HttpContext.Request.Method
        };
    }

    public object Action { get; set; }
}