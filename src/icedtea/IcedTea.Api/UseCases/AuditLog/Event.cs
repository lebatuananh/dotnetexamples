using AuditLogging.Events;
using Shared.Audit;

namespace IcedTea.Api.UseCases.AuditLog;

public class ApiAuditLogAuditEventBase : AuditEvent
{
    protected ApiAuditLogAuditEventBase()
    {
        Category = "AuditLog";
    }
}

public class ApiListAuditLogRequestEvent : ApiAuditLogAuditEventBase
{
    public ApiListAuditLogRequestEvent(string? @event, string? source, string? category, string? subjectIdentifier,
        string? subjectName, DateTimeOffset? created, int skip, int take, QueryResult<AuditLogDto> auditLogQueryResult)
    {
        Event = @event;
        Source = source;
        Category = category;
        SubjectIdentifier = subjectIdentifier;
        SubjectName = subjectName;
        Created = created;
        Skip = skip;
        Take = take;
        AuditLogQueryResult = auditLogQueryResult;
    }

    private string? Event { get; set; }
    private string? Source { get; set; }
    private string? Category { get; set; }
    private string? SubjectIdentifier { get; set; }
    private string? SubjectName { get; set; }
    private DateTimeOffset? Created { get; set; }
    private int Skip { get; init; }
    private int Take { get; init; }
    private QueryResult<AuditLogDto> AuditLogQueryResult { get; }
}

public class DeleteLogsOlderThanRequestEvent : ApiAuditLogAuditEventBase
{
    public DeleteLogsOlderThanRequestEvent(DateTime deleteOlderThan = default)
    {
        DeleteOlderThan = deleteOlderThan;
    }

    private DateTime DeleteOlderThan { get; set; }
}