using System.Reflection;

namespace IcedTea.Api.UseCases.AuditLog;

public record AuditLogDto(long Id, string Event, string Source, string Category, string SubjectIdentifier,
    string SubjectName, string SubjectType, string SubjectAdditionalData, string Action, DateTimeOffset Created);

public record AuditLogFilterDto(string Event, string Source, string Category, DateTime? Created,
    string SubjectIdentifier, string SubjectName, int Take = 10, int Skip = 0);