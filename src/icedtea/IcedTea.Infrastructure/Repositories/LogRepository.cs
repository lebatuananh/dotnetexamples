using System.Linq.Expressions;
using IcedTea.Domain.AggregateModel.LogErrorAggregate;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;
using Shared.Repositories;

namespace IcedTea.Infrastructure.Repositories;

public class LogRepository : ILogRepository
{
    protected readonly LogDbContext LogDbContext;

    public LogRepository(LogDbContext logDbContext)
    {
        LogDbContext = logDbContext;
    }

    public bool AutoSaveChanges { get; set; } = true;

    protected virtual async Task<int> AutoSaveChangesAsync()
    {
        return AutoSaveChanges ? await LogDbContext.SaveChangesAsync() : (int)SavedStatus.WillBeSavedExplicitly;
    }

    public async Task<QueryResult<Log>> GetLogsAsync(string? query, int skip = 0, int take = 10)
    {
        QueryResult<Log> queryable;
        if (!string.IsNullOrEmpty(query))
        {
            Expression<Func<Log, bool>> searchCondition = x =>
                x.LogEvent.Contains(query) || x.Message.Contains(query) ||
                x.Exception != null && x.Exception.Contains(query);
            queryable = await LogDbContext.Logs.Where(searchCondition)
                .OrderByDescending(x => x.TimeStamp).ToQueryResultAsync(skip, take);
            return queryable;
        }

        queryable = await LogDbContext.Logs
            .OrderByDescending(x => x.TimeStamp).ToQueryResultAsync(skip, take);
        return queryable;
    }

    public async Task DeleteLogsOlderThanAsync(DateTime deleteOlderThan)
    {
        var logsToDelete = await LogDbContext.Logs.Where(x => x.TimeStamp < deleteOlderThan.Date).ToListAsync();

        if (logsToDelete.Count == 0) return;

        LogDbContext.Logs.RemoveRange(logsToDelete);

        await AutoSaveChangesAsync();
    }
}