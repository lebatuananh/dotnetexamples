using AuditLogging.Events;

namespace IcedTea.Api.UseCases.CashFund;

public class ApiCashFundAuditEventBase : AuditEvent
{
    protected ApiCashFundAuditEventBase()
    {
        Category = "CashFund";
    }
}

