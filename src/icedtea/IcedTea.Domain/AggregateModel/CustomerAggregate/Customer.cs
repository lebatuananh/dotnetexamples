using IcedTea.Domain.AggregateModel.TransactionAggregate;
using Shared.SeedWork;

namespace IcedTea.Domain.AggregateModel.CustomerAggregate;

public class Customer : ModifierTrackingEntity, IAggregateRoot
{
    public string Name { get; private set; }
    public string UserName { get; private set; }
    public Guid WalletId { get; private set; }
    public Guid ExternalId { get; private set; }
    public int Status { get; private set; }
    public string DeviceId { get; private set; }
    public virtual Wallet Wallet { get; set; }
    public virtual IList<Transaction> Transactions { get; private set; }


    public Customer(string name, string userName, Guid walletId, Guid externalId, int status, string deviceId)
    {
        Name = name;
        UserName = userName;
        WalletId = walletId;
        ExternalId = externalId;
        Status = status;
        DeviceId = deviceId;
    }

    public void Update(string name, int status)
    {
        Name = name;
        Status = status;
    }

    public void UpdateWallet(Guid walletId, Wallet wallet)
    {
        if (walletId == WalletId)
        {
            Wallet.Update(wallet.TotalAmount, wallet.SubTotalAmount);
        }
        else
        {
            throw new ArgumentNullException("Wallet not found");
        }
    }

    public void Enable()
    {
        Status = 1;
    }

    public void Disable()
    {
        Status = 0;
    }

    public void Deposit(decimal amount)
    {
        Wallet.Deposit(amount);
    }

    public void Charge(decimal amount)
    {
        Wallet.Charge(amount);
    }

    public void Debt(decimal amount)
    {
        Wallet.Debt(amount);
    }
}