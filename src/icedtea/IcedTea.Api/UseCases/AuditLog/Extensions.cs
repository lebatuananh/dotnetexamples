using Microsoft.AspNetCore.Authorization;

namespace IcedTea.Api.UseCases.AuditLog;

public static class Extensions
{
    public static WebApplication UseAuditLogEndpoint(this WebApplication app)
    {
        app.MapGet("api/v1/admin/audit-logs",
                [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
                async ([FromQuery] string? @event, string? source, string? category, DateTime? created,
                        string? subjectIdentifier, string? subjectName, int skip, int take,
                        [FromServices] ISender sender) =>
                    await sender.Send(new MutateAuditLog.GetListAuditLogQueries(@event, source, category, created,
                        subjectIdentifier, subjectName, take, skip)))
            .Produces(200, typeof(ResultModel<QueryResult<AuditLogDto>>))
            .WithTags("AuditLog")
            .ProducesProblem(404);

        app.MapDelete("api/v1/admin/audit-logs",
                [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
                async ([FromBody] MutateAuditLog.DeleteAuditLogCommand command, ISender sender) =>
                    await sender.Send(command))
            .Produces(200, typeof(IResult))
            .WithTags("AuditLog")
            .ProducesProblem(404);

        return app;
    }
}