using System;
using System.Threading.Tasks;
using Shared.Repositories;

namespace Shared.Logging.LogError;

public interface ILogRepository
{
    bool AutoSaveChanges { get; set; }
    Task<QueryResult<Log>> GetLogsAsync(string? query, int skip = 1, int take = 10);

    Task DeleteLogsOlderThanAsync(DateTime deleteOlderThan);
}