using Shared.SeedWork;

namespace IcedTea.Domain.AggregateModel.CustomerAggregate;

public class Wallet : DateTrackingEntity
{
    public decimal TotalAmount { get; set; }
    public decimal SubTotalAmount { get; set; }
    public virtual Customer Customer { get; set; }

    public Wallet()
    {
    }

    public void Deposit(decimal amount)
    {
        TotalAmount += amount;
    }

    public void Charge(decimal amount)
    {
        TotalAmount -= amount;
    }

    public void Debt(decimal amount)
    {
        SubTotalAmount += amount;
    }

    public void Update(decimal totalAmount, decimal subTotalAmount)
    {
        TotalAmount = totalAmount;
        SubTotalAmount = subTotalAmount;
    }
}