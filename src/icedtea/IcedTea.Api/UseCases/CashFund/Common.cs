using IcedTea.Api.UseCases.Customer;
using IcedTea.Domain.AggregateModel.CashFundTransactionAggregate;
using IcedTea.Domain.AggregateModel.TransactionAggregate;

namespace IcedTea.Api.UseCases.CashFund;

public record CashFundDto(Guid Id, string Name, decimal TotalAmount, DateTimeOffset CreatedDate,
    DateTimeOffset LastUpdatedDate)
{
    public List<CashFundTransactionDto> CashFundTransactionDtos { set; get; }
    public List<TransactionDto> TransactionDtos { set; get; }

    public CashFundDto AssignTransaction(IList<CashFundTransaction> cashFundTransactions,
        IList<Transaction> transactions)
    {
        if (cashFundTransactions is { Count: > 0 })
        {
            CashFundTransactionDtos = cashFundTransactions.Select(x =>
                    new CashFundTransactionDto(x.Id, x.TotalAmount, x.Note, x.PaymentGateway, x.CompletedDate,
                        x.Status))
                .ToList();
        }

        if (transactions is { Count: > 0 })
        {
            TransactionDtos = transactions.Select(x => new TransactionDto(x.Id, x.TotalAmount, x.Note, x.ErrorMessage,
                x.BankAccount, x.CompletedDate, x.Response, x.PaymentGateway, x.Status)).ToList();
        }


        return this;
    }
}

public record CashFundTransactionDto(Guid Id, decimal TotalAmount, string Note, PaymentGateway PaymentGateway,
    DateTimeOffset CompletedDate, TransactionStatus Status);