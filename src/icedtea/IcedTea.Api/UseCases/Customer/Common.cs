using IcedTea.Domain.AggregateModel.TransactionAggregate;

namespace IcedTea.Api.UseCases.Customer;

public record CustomerDto(Guid Id, string Name, string UserName, string DeviceId, int Status,
    DateTimeOffset CreatedDate,
    DateTimeOffset LastUpdatedDate,
    WalletDto WalletDto)
{
    public List<TransactionDto> TransactionDto { get; set; }

    public CustomerDto AssignTransactions(IList<Transaction> transactions)
    {
        if (transactions is { Count: > 0 })
        {
            TransactionDto = transactions.Select(x => new TransactionDto(x.Id, x.TotalAmount, x.Note, x.ErrorMessage,
                x.BankAccount, x.CompletedDate, x.Response, x.PaymentGateway, x.Status)).ToList();
        }

        return this;
    }
}

public record WalletDto(decimal TotalAmount, decimal SubTotalAmount);

public record TransactionDto(Guid Id, decimal TotalAmount, string Note, string ErrorMessage, string BankAccount,
    DateTimeOffset CompletedDate, string Response, PaymentGateway PaymentGateway, TransactionStatus Status);