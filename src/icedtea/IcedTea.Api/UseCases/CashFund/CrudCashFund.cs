using System.Linq.Expressions;
using IcedTea.Domain.AggregateModel.CashFundAggregate;
using CashFundEntity = IcedTea.Domain.AggregateModel.CashFundAggregate.CashFund;

namespace IcedTea.Api.UseCases.CashFund;

public struct MutateCashFund
{
    public record GetListCashFundQueries(int Skip, int Take, string? Query) : IQueries;

    public record GetCashFundQuery : IQuery
    {
        public Guid Id { get; init; }

        internal class GetSpec : SpecificationBase<CashFundEntity>
        {
            private readonly Guid _id;

            public GetSpec(Guid id)
            {
                _id = id;
                Includes.Add(x => x.Transactions);
            }

            public override Expression<Func<CashFundEntity, bool>> Criteria => x => x.Id == _id;

            internal class GetValidator : AbstractValidator<GetCashFundQuery>
            {
                public GetValidator()
                {
                    RuleFor(v => v.Id)
                        .NotEmpty()
                        .WithMessage("Id is required.");
                }
            }
        }
    }

    public record CreateCashFundCommand : ICreateCommand
    {
        public string Name { get; init; }

        public CashFundEntity ToCashFundEntity()
        {
            return new CashFundEntity(Name, 0);
        }

        internal class Validator : AbstractValidator<CreateCashFundCommand>
        {
            public Validator()
            {
                RuleFor(x => x.Name)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Name is not empty");
            }
        }
    }


    public record UpdateCashFundCommand : IUpdateCommand<Guid>
    {
        public Guid Id { get; init; }
        public string Name { get; init; }


        internal class Validator : AbstractValidator<UpdateCashFundCommand>
        {
            public Validator()
            {
                RuleFor(x => x.Name)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Name is not empty");
                RuleFor(x => x.Id)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Id is not empty");
            }
        }
    }

    internal class Handler : IRequestHandler<GetListCashFundQueries, IResult>,
        IRequestHandler<GetCashFundQuery, IResult>,
        IRequestHandler<CreateCashFundCommand, IResult>,
        IRequestHandler<UpdateCashFundCommand, IResult>
    {
        private readonly ICashFundRepository _cashFundRepository;

        public Handler(ICashFundRepository cashFundRepository)
        {
            _cashFundRepository = cashFundRepository;
        }

        public async Task<IResult> Handle(GetListCashFundQueries request, CancellationToken cancellationToken)
        {
            var queryable = await _cashFundRepository
                .FindAll(x => string.IsNullOrEmpty(request.Query)
                              || EF.Functions.ILike(x.Name, $"%{request.Query}%")
                )
                .OrderByDescending(x => x.CreatedDate).ToQueryResultAsync(request.Skip, request.Take);
            var customerModels = new QueryResult<CashFundDto>
            {
                Count = queryable.Count,
                Items = queryable.Items
                    .Select(x => new CashFundDto(x.Id, x.Name, x.TotalAmount))
                    .ToList()
            };
            return Results.Ok(ResultModel<QueryResult<CashFundDto>>.Create(customerModels));
        }

        public async Task<IResult> Handle(GetCashFundQuery request, CancellationToken cancellationToken)
        {
            var item = await _cashFundRepository.GetByIdAsync(request.Id, c => c.Transactions);
            if (item is null) throw new Exception($"Couldn't find item={request.Id}");
            var result = new CashFundDto(item.Id, item.Name, item.TotalAmount);
            result.AssignTransaction(item.CashFundTransactions, item.Transactions);
            return Results.Ok(ResultModel<CashFundDto>.Create(result));
        }

        public async Task<IResult> Handle(CreateCashFundCommand request, CancellationToken cancellationToken)
        {
            var cashFundEntity = request.ToCashFundEntity();
            _cashFundRepository.Add(cashFundEntity);
            await _cashFundRepository.CommitAsync();
            return Results.Ok();
        }

        public async Task<IResult> Handle(UpdateCashFundCommand request, CancellationToken cancellationToken)
        {
            var item = await _cashFundRepository.GetByIdAsync(request.Id);
            if (item is null) throw new Exception($"Couldn't find entity with id={request.Id}");
            item.Update(request.Name);
            _cashFundRepository.Update(item);
            await _cashFundRepository.CommitAsync();
            return Results.Ok();
        }
    }
}