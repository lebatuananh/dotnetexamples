using System.Linq.Expressions;
using IcedTea.Domain.AggregateModel.CustomerAggregate;
using CustomerEntity = IcedTea.Domain.AggregateModel.CustomerAggregate.Customer;

namespace IcedTea.Api.UseCases.Customer;

public struct MutateCustomer
{
    public record GetListCustomerQueries(int Skip, int Take, string? Query) : IQueries;

    public record GetCustomerQuery : IQuery
    {
        public Guid Id { get; init; }

        internal class GetSpec : SpecificationBase<CustomerEntity>
        {
            private readonly Guid _id;

            public GetSpec(Guid id)
            {
                _id = id;
                Includes.Add(x => x.Wallet);
                Includes.Add(x => x.Transactions);
            }

            public override Expression<Func<CustomerEntity, bool>> Criteria => x => x.Id == _id;

            internal class GetValidator : AbstractValidator<GetCustomerQuery>
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

    public record CreateCustomerCommand : ICreateCommand
    {
        public string Name { get; init; }
        public string UserName { get; init; }
        public int Status { get; init; }
        public string DeviceId { get; init; }
        public Guid ExternalId { get; init; }

        public CustomerEntity ToCustomerEntity()
        {
            return new CustomerEntity(Name, UserName, ExternalId, Status, DeviceId);
        }

        internal class Validator : AbstractValidator<CreateCustomerCommand>
        {
            public Validator()
            {
                RuleFor(x => x.Name)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Name is not empty");
                RuleFor(x => x.UserName)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("UserName is not empty");
            }
        }
    }


    public record UpdateCustomerCommand : IUpdateCommand<Guid>
    {
        public Guid Id { get; init; }
        public string Name { get; init; }


        internal class Validator : AbstractValidator<UpdateCustomerCommand>
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

    internal class Handler : IRequestHandler<GetListCustomerQueries, IResult>,
        IRequestHandler<GetCustomerQuery, IResult>,
        IRequestHandler<CreateCustomerCommand, IResult>,
        IRequestHandler<UpdateCustomerCommand, IResult>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IScopeContext _scopeContext;

        public Handler(ICustomerRepository customerRepository, IScopeContext scopeContext)
        {
            _customerRepository = customerRepository;
            _scopeContext = scopeContext;
        }

        public async Task<IResult> Handle(GetListCustomerQueries request, CancellationToken cancellationToken)
        {
            var customerModels = new QueryResult<CustomerDto>();
            if (_scopeContext.CurrentAccountId != Guid.Empty && _scopeContext.Role == AuthorizationConsts.AdminRole)
            {
                var queryable = await _customerRepository
                    .FindAll(x => string.IsNullOrEmpty(request.Query)
                                  || EF.Functions.ILike(x.Name, $"%{request.Query}%")
                    )
                    .OrderByDescending(x => x.CreatedDate).ToQueryResultAsync(request.Skip, request.Take);
                customerModels = new QueryResult<CustomerDto>
                {
                    Count = queryable.Count,
                    Items = queryable.Items
                        .Select(x => new CustomerDto(x.Id, x.Name, x.UserName, x.DeviceId, x.Status, x.CreatedDate,
                            x.LastUpdatedDate, new WalletDto(x.Wallet.TotalAmount, x.Wallet.SubTotalAmount)))
                        .ToList()
                };
            }
            else
            {
                
            }

            return Results.Ok(ResultModel<QueryResult<CustomerDto>>.Create(customerModels));
        }

        public async Task<IResult> Handle(GetCustomerQuery request, CancellationToken cancellationToken)
        {
            var item = await _customerRepository.GetByIdAsync(request.Id, c => c.Transactions);
            if (item is null) throw new Exception($"Couldn't find item={request.Id}");
            var result = new CustomerDto(item.Id, item.Name, item.UserName, item.DeviceId, item.Status,
                item.CreatedDate,
                item.LastUpdatedDate, new WalletDto(item.Wallet.TotalAmount, item.Wallet.SubTotalAmount));
            result.AssignTransactions(item.Transactions);
            return Results.Ok(ResultModel<CustomerDto>.Create(result));
        }

        public async Task<IResult> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            var customerEntity = request.ToCustomerEntity();
            _customerRepository.Add(customerEntity);
            await _customerRepository.CommitAsync();
            return Results.Ok();
        }

        public async Task<IResult> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            var item = await _customerRepository.GetByIdAsync(request.Id);
            if (item is null) throw new Exception($"Couldn't find entity with id={request.Id}");

            item.Update(request.Name);
            _customerRepository.Update(item);
            await _customerRepository.CommitAsync();
            return Results.Ok();
        }
    }
}