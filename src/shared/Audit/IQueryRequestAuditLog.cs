namespace Shared.Audit;

public interface IQueryRequestAuditLog
{
    public int Skip { get; init; }
    public int Take { get; init; }
    public string? Query { get; init; }
}