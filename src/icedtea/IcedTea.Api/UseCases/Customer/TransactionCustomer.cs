﻿using IcedTea.Domain.AggregateModel.CashFundAggregate;
using IcedTea.Domain.AggregateModel.CashFundTransactionAggregate;
using IcedTea.Domain.AggregateModel.CustomerAggregate;
using IcedTea.Domain.AggregateModel.TransactionAggregate;
using TransactionEntity = IcedTea.Domain.AggregateModel.TransactionAggregate.Transaction;

namespace IcedTea.Api.UseCases.Customer;

public struct TransactionCustomer
{
    public record CustomerDepositCommand(Guid Id) : IUpdateCommand<Guid>
    {
        public decimal TotalAmount { get; init; }
        public string Note { get; init; }
        public string BankAccount { get; init; }
        public PaymentGateway PaymentGateway { get; init; }

        public Transaction ToTransaction()
        {
            return new Transaction(Id, TotalAmount, Note, BankAccount, PaymentGateway);
        }

        internal class Validator : AbstractValidator<CustomerDepositCommand>
        {
            public Validator()
            {
                RuleFor(x => x.TotalAmount)
                    .GreaterThan(0)
                    .WithMessage("TotalAmount is greater than 0");
                RuleFor(x => x.PaymentGateway)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("PaymentGateway is not empty");
                RuleFor(x => x.Id)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Id is not empty");
            }
        }
    }

    public record AcceptTransactionCustomerDepositCommand(Guid Id) : IUpdateCommand<Guid>
    {
        public Guid TransactionId { get; init; }


        internal class Validator : AbstractValidator<AcceptTransactionCustomerDepositCommand>
        {
            public Validator()
            {
                RuleFor(x => x.Id)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Id is not empty");

                RuleFor(x => x.TransactionId)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("TransactionId is not empty");
            }
        }
    }

    public record CustomerDepositCashFundCommand(Guid Id) : IUpdateCommand<Guid>
    {
        public decimal TotalAmount { get; init; }
        public string Note { get; init; }
        public PaymentGateway PaymentGateway { get; init; }
        public Guid CashFundId { get; set; }
        public string CustomerName { get; set; }

        public CashFundTransaction ToCashFundTransaction()
        {
            return new CashFundTransaction(TotalAmount, Note, PaymentGateway, CashFundId, Id, CustomerName);
        }

        internal class Validator : AbstractValidator<CustomerDepositCashFundCommand>
        {
            public Validator()
            {
                RuleFor(x => x.Id)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Id is not empty");

                RuleFor(x => x.CashFundId)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("CashFundId is not empty");

                RuleFor(x => x.TotalAmount)
                    .GreaterThan(0)
                    .WithMessage("TotalAmount is greater than 0");
            }
        }
    }

    internal class Handler : IRequestHandler<CustomerDepositCommand, IResult>,
        IRequestHandler<AcceptTransactionCustomerDepositCommand, IResult>,
        IRequestHandler<CustomerDepositCashFundCommand, IResult>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICashFundRepository _cashFundRepository;

        public Handler(ICustomerRepository customerRepository, ITransactionRepository transactionRepository,
            ICashFundRepository cashFundRepository)
        {
            _customerRepository = customerRepository;
            _transactionRepository = transactionRepository;
            _cashFundRepository = cashFundRepository;
        }

        public async Task<IResult> Handle(CustomerDepositCommand request, CancellationToken cancellationToken)
        {
            var item = await _customerRepository.GetByIdAsync(request.Id);
            if (item is null) throw new Exception($"Couldn't find entity with id={request.Id}");
            var transaction = request.ToTransaction();
            transaction.MarkCompleted();
            _transactionRepository.Add(transaction);
            await _customerRepository.CommitAsync();
            return Results.Ok();
        }

        public async Task<IResult> Handle(AcceptTransactionCustomerDepositCommand request,
            CancellationToken cancellationToken)
        {
            var item = await _customerRepository.GetByIdAsync(request.Id, c => c.Wallet);
            if (item is null) throw new Exception($"Couldn't find entity with id={request.Id}");
            var itemTransaction = await _transactionRepository.GetByIdAsync(request.TransactionId);
            if (itemTransaction is null) throw new Exception($"Couldn't find entity with id={request.TransactionId}");
            if (!itemTransaction.Status.Equals(TransactionStatus.Completed)) return Results.Ok();
            itemTransaction.MarkAccept();
            item.Deposit(itemTransaction.TotalAmount);
            await _customerRepository.CommitAsync();
            return Results.Ok();
        }

        public async Task<IResult> Handle(CustomerDepositCashFundCommand request, CancellationToken cancellationToken)
        {
            var item = await _customerRepository.GetByIdAsync(request.Id, c => c.Wallet);
            if (item is null) throw new Exception($"Couldn't find entity with id={request.Id}");
            if (item.Wallet.TotalAmount <= 0 || item.Wallet.TotalAmount < request.TotalAmount)
                throw new Exception($"Amount is not enough to make this transaction ");
            {
                var itemCashFund =
                    await _cashFundRepository.GetByIdAsync(request.CashFundId, c => c.CashFundTransactions);
                if (item is null) throw new Exception($"Couldn't find entity with id={request.CashFundId}");
                var cashFundTransaction = request.ToCashFundTransaction();
                cashFundTransaction.MarkAccept();
                itemCashFund.Deposit(request.TotalAmount, cashFundTransaction);
                item.Charge(request.TotalAmount);
                await _customerRepository.CommitAsync();
                return Results.Ok();
            }
        }
    }
}