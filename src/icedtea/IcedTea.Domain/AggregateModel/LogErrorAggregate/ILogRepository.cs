using Shared.Repositories;

namespace IcedTea.Domain.AggregateModel.LogErrorAggregate;

public interface ILogRepository
{
    bool AutoSaveChanges { get; set; }
    Task<QueryResult<Log>> GetLogsAsync(string? query, int skip = 1, int take = 10);

    Task DeleteLogsOlderThanAsync(DateTime deleteOlderThan);
}