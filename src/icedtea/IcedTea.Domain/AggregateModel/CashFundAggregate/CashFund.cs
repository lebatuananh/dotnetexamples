using IcedTea.Domain.AggregateModel.CashFundTransactionAggregate;
using IcedTea.Domain.AggregateModel.TransactionAggregate;
using Shared.SeedWork;

namespace IcedTea.Domain.AggregateModel.CashFundAggregate;

public class CashFund : ModifierTrackingEntity, IAggregateRoot
{
    public string Name { get; set; }
    public decimal TotalAmount { get; private set; }
    public virtual IList<Transaction> Transactions { get; private set; }
    public virtual IList<CashFundTransaction> CashFundTransactions { get; private set; }


    public CashFund()
    {
    }

    public void Update(string name)
    {
        Name = name;
    }

    public CashFund(string name, decimal totalAmount)
    {
        Name = name;
        TotalAmount = totalAmount;
    }

    public void Deposit(decimal amount, CashFundTransaction cashFundTransaction)
    {
        CashFundTransactions ??= new List<CashFundTransaction>();
        CashFundTransactions.Add(cashFundTransaction);
        TotalAmount += amount;
    }
    

    public void Charge(decimal amount, CashFundTransaction cashFundTransaction)
    {
        CashFundTransactions ??= new List<CashFundTransaction>();
        CashFundTransactions.Add(cashFundTransaction);
        TotalAmount -= amount;
    }
}