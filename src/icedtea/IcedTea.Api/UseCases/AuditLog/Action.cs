using Shared.Audit.Repository;
using AuditLogEntity = AuditLogging.EntityFramework.Entities.AuditLog;

namespace IcedTea.Api.UseCases.AuditLog;

public struct MutateAuditLog
{
    public record GetListAuditLogQueries(string? Event, string? Source, string? Category, DateTime? Created,
        string? SubjectIdentifier, string? SubjectName, int Take = 10, int Skip = 0) : IQuery;

    public record DeleteAuditLogCommand(DateTime DeleteOlderThan) : ICommand;

    internal class Handler : IRequestHandler<GetListAuditLogQueries, IResult>,
        IRequestHandler<DeleteAuditLogCommand, IResult>
    {
        private readonly IAuditLogRepository<AuditLogEntity> _auditLogRepository;

        public Handler(IAuditLogRepository<AuditLogEntity> auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        public async Task<IResult> Handle(GetListAuditLogQueries request, CancellationToken cancellationToken)
        {
            var queryable = await _auditLogRepository.GetAsync(request.Event, request.Source,
                request.Category,
                request.Created, request.SubjectIdentifier, request.SubjectName, request.Skip,
                request.Take);
            var logErrorModels = new QueryResult<AuditLogDto>
            {
                Count = queryable.Count,
                Items = queryable.Items
                    .Select(x => new AuditLogDto(x.Id, x.Event, x.Source, x.Category, x.SubjectIdentifier,
                        x.SubjectName, x.SubjectType, x.SubjectAdditionalData, x.Action, x.Created))
                    .ToList()
            };
            return Results.Ok(ResultModel<QueryResult<AuditLogDto>>.Create(logErrorModels));
        }

        public async Task<IResult> Handle(DeleteAuditLogCommand request, CancellationToken cancellationToken)
        {
            await _auditLogRepository.DeleteLogsOlderThanAsync(request.DeleteOlderThan);
            return Results.Ok();
        }
    }
}