using IcedTea.Domain.AggregateModel.CashFundAggregate;
using IcedTea.Domain.AggregateModel.TransactionAggregate;
using Shared.SeedWork;

namespace IcedTea.Domain.AggregateModel.CashFundTransactionAggregate;

public class CashFundTransaction : ModifierTrackingEntity, IAggregateRoot
{
    public decimal TotalAmount { get; private set; }
    public string Note { get; private set; }
    public PaymentGateway PaymentGateway { get; private set; }
    public DateTimeOffset CompletedDate { get; private set; }
    public TransactionStatus Status { get; private set; }
    public Guid CashFundId { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; }
    public virtual CashFund CashFund { get; private set; }

    public CashFundTransaction()
    {
    }

    public CashFundTransaction(decimal totalAmount, string note,
        PaymentGateway paymentGateway, Guid cashFundId) : this()
    {
        TotalAmount = totalAmount;
        Note = note;
        PaymentGateway = paymentGateway;
        CashFundId = cashFundId;
    }

    public CashFundTransaction(decimal totalAmount, string note,
        PaymentGateway paymentGateway, Guid cashFundId, Guid customerId, string customerName) : this()
    {
        TotalAmount = totalAmount;
        Note = note;
        PaymentGateway = paymentGateway;
        CustomerId = customerId;
        CustomerName = customerName;
        CashFundId = cashFundId;
    }

    public void MarkCompleted()
    {
        CompletedDate = DateTimeOffset.UtcNow;
        Status = TransactionStatus.Completed;
    }

    public void MarkAccept()
    {
        CompletedDate = DateTimeOffset.UtcNow;
        Status = TransactionStatus.Accept;
    }
}