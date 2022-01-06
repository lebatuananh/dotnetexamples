using AuditLogging.Services;
using IcedTea.Domain.AggregateModel.CashFundAggregate;
using IcedTea.Domain.AggregateModel.CashFundTransactionAggregate;
using IcedTea.Domain.AggregateModel.TransactionAggregate;

namespace IcedTea.Api.UseCases.CashFund;

public struct TransactionCashFund
{
    public record ChargeCashFundCommand : IUpdateCommand<Guid>
    {
        public Guid Id { get; init; }
        public decimal TotalAmount { get; init; }
        public string Note { get; init; }
        public PaymentGateway PaymentGateway { get; init; }
        public string CustomerName { get; set; }

        public CashFundTransaction ToCashFundTransaction()
        {
            return new CashFundTransaction(TotalAmount, Note, PaymentGateway, Id, Guid.Empty, CustomerName);
        }


        internal class Validator : AbstractValidator<ChargeCashFundCommand>
        {
            public Validator()
            {
                RuleFor(x => x.Id)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Id is not empty");


                RuleFor(x => x.TotalAmount)
                    .GreaterThan(0)
                    .WithMessage("TotalAmount is greater than 0");
            }
        }
    }

    internal class Handler : IRequestHandler<ChargeCashFundCommand, IResult>
    {
        private readonly ICashFundRepository _cashFundRepository;
        private readonly IAuditEventLogger _auditEventLogger;

        public Handler(ICashFundRepository cashFundRepository, IAuditEventLogger auditEventLogger)
        {
            _cashFundRepository = cashFundRepository;
            _auditEventLogger = auditEventLogger;
        }

        public async Task<IResult> Handle(ChargeCashFundCommand request, CancellationToken cancellationToken)
        {
            var itemCashFund =
                await _cashFundRepository.GetByIdAsync(request.Id, c => c.CashFundTransactions);

            if (itemCashFund is not { TotalAmount: > 0 } || itemCashFund.TotalAmount < request.TotalAmount)
                throw new Exception($"Amount is not enough to make this transaction ");
            var cashFundTransaction = request.ToCashFundTransaction();
            cashFundTransaction.MarkAccept();
            itemCashFund.Charge(request.TotalAmount, cashFundTransaction);
            await _cashFundRepository.CommitAsync();
            await _auditEventLogger.LogEventAsync(new ApiChargeCashFundRequestEvent(request.Id, request.TotalAmount,
                request.Note, request.PaymentGateway, request.CustomerName, itemCashFund));
            return Results.Ok();
        }
    }
}