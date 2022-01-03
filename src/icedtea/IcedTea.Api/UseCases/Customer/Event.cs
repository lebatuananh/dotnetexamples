using AuditLogging.Events;

namespace IcedTea.Api.UseCases.Customer;

public class ApiCustomerAuditEventBase : AuditEvent
{
    protected ApiCustomerAuditEventBase()
    {
        Category = "Customer";
    }
}

public class ApiCustomerRequestEvent : ApiCustomerAuditEventBase
{
    public ApiCustomerRequestEvent(QueryResult<CustomerDto> customerQueryResult)
    {
        CustomerQueryResult = customerQueryResult;
    }

    public QueryResult<CustomerDto> CustomerQueryResult { get; set; }
}