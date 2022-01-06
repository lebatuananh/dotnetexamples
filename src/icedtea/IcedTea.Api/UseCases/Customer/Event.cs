using AuditLogging.Events;
using IcedTea.Domain.AggregateModel.TransactionAggregate;
using Shared.Audit;
using CustomerEntity = IcedTea.Domain.AggregateModel.CustomerAggregate.Customer;

namespace IcedTea.Api.UseCases.Customer;

public class ApiCustomerAuditEventBase : AuditEvent
{
    protected ApiCustomerAuditEventBase()
    {
        Category = "Customer";
    }
}

public class ApiListCustomerRequestEvent : ApiCustomerAuditEventBase, IQueryRequestAuditLog
{
    public ApiListCustomerRequestEvent(QueryResult<CustomerDto> customerQueryResult, int skip, int take, string? query)
    {
        CustomerQueryResult = customerQueryResult;
        Skip = skip;
        Take = take;
        Query = query;
    }

    private QueryResult<CustomerDto> CustomerQueryResult { get; }
    public int Skip { get; init; }
    public int Take { get; init; }
    public string? Query { get; init; }
}

public class ApiCustomerRequestEvent : ApiCustomerAuditEventBase
{
    public ApiCustomerRequestEvent(CustomerDto customerDto)
    {
        CustomerDto = customerDto;
    }

    private CustomerDto CustomerDto { get; }
}

public class ApiCreateCustomerRequestEvent : ApiCustomerAuditEventBase
{
    public ApiCreateCustomerRequestEvent(string name, string userName, string email, string phoneNumber,
        string password, string confirmPassword, int status, string deviceId, Guid externalId)
    {
        Name = name;
        UserName = userName;
        Email = email;
        PhoneNumber = phoneNumber;
        Password = password.MD5();
        ConfirmPassword = confirmPassword.MD5();
        Status = status;
        DeviceId = deviceId;
        ExternalId = externalId;
    }

    private string Name { get; }
    private string UserName { get; }
    private string Email { get; }
    private string PhoneNumber { get; }
    private string Password { get; }
    private string ConfirmPassword { get; }
    private int Status { get; }
    private string DeviceId { get; }
    private Guid ExternalId { get; set; }
}

public class ApiUpdateCustomerRequestEvent : ApiCustomerAuditEventBase
{
    public ApiUpdateCustomerRequestEvent(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    private Guid Id { get; }
    private string Name { get; }
}

public class ApiUpdateDeviceCustomerRequestEvent : ApiCustomerAuditEventBase
{
    public ApiUpdateDeviceCustomerRequestEvent(Guid id, string deviceId)
    {
        Id = id;
        DeviceId = deviceId;
    }

    private Guid Id { get; }
    private string DeviceId { get; }
}

public class ApiChangePasswordRequestEvent : ApiCustomerAuditEventBase
{
    public ApiChangePasswordRequestEvent(string password, string confirmPassword)
    {
        Password = password.MD5();
        ConfirmPassword = confirmPassword.MD5();
    }

    private string Password { get; }
    private string ConfirmPassword { get; }
}

public class ApiChangePasswordUserRequestEvent : ApiCustomerAuditEventBase
{
    public ApiChangePasswordUserRequestEvent(string password, string confirmPassword, Guid id)
    {
        Password = password.MD5();
        ConfirmPassword = confirmPassword.MD5();
        Id = id;
    }

    private Guid Id { get; }
    private string Password { get; }
    private string ConfirmPassword { get; }
}

public class ApiAssignRoleRequestEvent : ApiCustomerAuditEventBase
{
    public ApiAssignRoleRequestEvent(Guid id, Guid roleId)
    {
        Id = id;
        RoleId = roleId;
    }

    private Guid Id { get; }
    private Guid RoleId { get; }
}

public class ApiDisableCustomerRequestEvent : ApiCustomerAuditEventBase
{
    public ApiDisableCustomerRequestEvent(Guid id, CustomerEntity originalCustomer)
    {
        Id = id;
        OriginalCustomer = originalCustomer;
    }

    private Guid Id { get; }
    private CustomerEntity OriginalCustomer { get; set; }
}

public class ApiEnableCustomerRequestEvent : ApiCustomerAuditEventBase
{
    public ApiEnableCustomerRequestEvent(Guid id, CustomerEntity originalCustomer)
    {
        Id = id;
        OriginalCustomer = originalCustomer;
    }

    private Guid Id { get; }
    private CustomerEntity OriginalCustomer { get; set; }
}

public class ApiListCustomerTransactionRequestEvent : ApiCustomerAuditEventBase, IQueryRequestAuditLog
{
    public ApiListCustomerTransactionRequestEvent(int skip, int take, string? query,
        QueryResult<TransactionDto> customerTransactionQueryResult)
    {
        Skip = skip;
        Take = take;
        Query = query;
        CustomerTransactionQueryResult = customerTransactionQueryResult;
    }

    public int Skip { get; init; }
    public int Take { get; init; }
    public string? Query { get; init; }
    private QueryResult<TransactionDto> CustomerTransactionQueryResult { get; }
}

public class ApiDepositCustomerRequestEvent : ApiCustomerAuditEventBase
{
    public ApiDepositCustomerRequestEvent(Guid id = default, decimal totalAmount = default, string? note = null,
        string? bankAccount = null, PaymentGateway paymentGateway = default)
    {
        Id = id;
        TotalAmount = totalAmount;
        Note = note ?? throw new ArgumentNullException(nameof(note));
        BankAccount = bankAccount ?? throw new ArgumentNullException(nameof(bankAccount));
        PaymentGateway = paymentGateway;
    }

    private Guid Id { get; }
    private decimal TotalAmount { get; init; }
    private string Note { get; init; }
    private string BankAccount { get; init; }
    private PaymentGateway PaymentGateway { get; init; }
}

public class ApiAcceptTransactionCustomerDepositRequestEvent : ApiCustomerAuditEventBase
{
    public ApiAcceptTransactionCustomerDepositRequestEvent(Guid id, Guid transactionId, Transaction originalTransaction)
    {
        Id = id;
        TransactionId = transactionId;
        OriginalTransaction = originalTransaction;
    }

    private Guid Id { get; }
    private Guid TransactionId { get; }
    private Transaction OriginalTransaction { get; }
}

public class ApiCustomerDepositCashFundRequestEvent : ApiCustomerAuditEventBase
{
    public ApiCustomerDepositCashFundRequestEvent(Guid id, decimal totalAmount, string note,
        PaymentGateway paymentGateway, Guid cashFundId, string customerName)
    {
        Id = id;
        TotalAmount = totalAmount;
        Note = note;
        PaymentGateway = paymentGateway;
        CashFundId = cashFundId;
        CustomerName = customerName;
    }

    private Guid Id { get; }
    private decimal TotalAmount { get; }
    private string Note { get; }
    private PaymentGateway PaymentGateway { get; }
    private Guid CashFundId { get; }
    private string CustomerName { get; }
}