using IcedTea.Domain.AggregateModel.LogErrorAggregate;

namespace IcedTea.Api.UseCases.LogError;

public struct MutateLogError
{
    public record GetListLogErrorQueries(int Skip, int Take, string? Query) : IQueries;

    public record DeleteLogErrorCommand(DateTime DeleteOlderThan) : ICommand;

    internal class Handler : IRequestHandler<GetListLogErrorQueries, IResult>,
        IRequestHandler<DeleteLogErrorCommand, IResult>
    {
        private readonly ILogRepository _logRepository;

        public Handler(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        public async Task<IResult> Handle(GetListLogErrorQueries request, CancellationToken cancellationToken)
        {
            var (skip, take, query) = request;
            var queryable = await _logRepository.GetLogsAsync(query, skip, take);
            var logErrorModels = new QueryResult<LogErrorDto>
            {
                Count = queryable.Count,
                Items = queryable.Items
                    .Select(x => new LogErrorDto(x.Id, x.Message, x.Level, x.TimeStamp, x.LogEvent, x.Properties))
                    .ToList()
            };
            return Results.Ok(ResultModel<QueryResult<LogErrorDto>>.Create(logErrorModels));
        }

        public async Task<IResult> Handle(DeleteLogErrorCommand request, CancellationToken cancellationToken)
        {
            await _logRepository.DeleteLogsOlderThanAsync(request.DeleteOlderThan);
            return Results.Ok();
        }
    }
}