using AuditLogging.Events;
using IcedTea.Domain.AggregateModel.TransactionAggregate;
using Shared.Audit;
using CashFundEntity = IcedTea.Domain.AggregateModel.CashFundAggregate.CashFund;

namespace IcedTea.Api.UseCases.CashFund;

public class ApiCashFundAuditEventBase : AuditEvent
{
    protected ApiCashFundAuditEventBase()
    {
        Category = "CashFund";
    }
}

public class ApiListCashFundRequestEvent : ApiCashFundAuditEventBase, IQueryRequestAuditLog
{
    public ApiListCashFundRequestEvent(QueryResult<CashFundDto> cashFundQueryResult, int skip, int take, string? query)
    {
        CashFundQueryResult = cashFundQueryResult;
        Skip = skip;
        Take = take;
        Query = query;
    }

    private QueryResult<CashFundDto> CashFundQueryResult { get; }
    public int Skip { get; init; }
    public int Take { get; init; }
    public string? Query { get; init; }
}

public class ApiCashFundRequestEvent : ApiCashFundAuditEventBase
{
    public ApiCashFundRequestEvent(Guid id, CashFundDto cashFundDto)
    {
        Id = id;
        CashFundDto = cashFundDto;
    }

    private Guid Id { get; }
    private CashFundDto CashFundDto { get; }
}

public class ApiUpdateCashFundRequestEvent : ApiCashFundAuditEventBase
{
    public ApiUpdateCashFundRequestEvent(Guid id, string name, CashFundEntity oldCashFund)
    {
        Id = id;
        Name = name;
        OldCashFund = oldCashFund;
    }

    private Guid Id { get; }
    private string Name { get; }
    private CashFundEntity OldCashFund { get; }
}

public class ApiCreateCashFundRequestEvent : ApiCashFundAuditEventBase
{
    public ApiCreateCashFundRequestEvent(string name)
    {
        Name = name;
    }

    private string Name { get; }
}

public class ApiChargeCashFundRequestEvent : ApiCashFundAuditEventBase
{
    public ApiChargeCashFundRequestEvent(Guid id, decimal totalAmount, string note, PaymentGateway paymentGateway,
        string customerName, CashFundEntity cashFund)
    {
        Id = id;
        TotalAmount = totalAmount;
        Note = note;
        PaymentGateway = paymentGateway;
        CustomerName = customerName;
        CashFund = cashFund;
    }

    public Guid Id { get; }
    public decimal TotalAmount { get; }
    public string Note { get; }
    public PaymentGateway PaymentGateway { get; }
    public string CustomerName { get; }
    private CashFundEntity CashFund { get; }
}