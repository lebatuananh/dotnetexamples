﻿using System.Linq.Expressions;
using AuditLogging.Services;
using IcedTea.Domain.AggregateModel.CustomerAggregate;
using User.Api;
using User.Api.Models;
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
        public string Email { get; init; }
        public string PhoneNumber { get; init; }
        public string Password { get; init; }
        public string ConfirmPassword { get; init; }
        public int Status { get; init; }
        public string DeviceId { get; init; }
        public Guid ExternalId { get; set; }

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
                RuleFor(x => x.Password)
                    .NotEmpty()
                    .NotNull()
                    .WithMessage("Password is not empty");
                RuleFor(x => x.ConfirmPassword)
                    .NotEmpty()
                    .NotNull()
                    .WithMessage("ConfirmPassword is not empty");
                RuleFor(customer => customer.Password)
                    .Equal(customer => customer.ConfirmPassword).WithMessage("Password miss match");
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
        private readonly IUserApi _userApi;
        private readonly IAuditEventLogger _auditEventLogger;

        public Handler(ICustomerRepository customerRepository, IScopeContext scopeContext, IUserApi userApi,
            IAuditEventLogger auditEventLogger)
        {
            _customerRepository = customerRepository;
            _scopeContext = scopeContext;
            _userApi = userApi;
            _auditEventLogger = auditEventLogger;
        }

        public async Task<IResult> Handle(GetListCustomerQueries request, CancellationToken cancellationToken)
        {
            QueryResult<CustomerDto> customerModels;
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
                var queryable = await _customerRepository
                    .FindAll(x => x.ExternalId.Equals(_scopeContext.CurrentAccountId))
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

            await _auditEventLogger.LogEventAsync(new ApiListCustomerRequestEvent(customerModels, request.Skip,
                request.Take, request.Query));
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
            await _auditEventLogger.LogEventAsync(new ApiCustomerRequestEvent(result));
            return Results.Ok(ResultModel<CustomerDto>.Create(result));
        }

        public async Task<IResult> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            var userId = Guid.NewGuid();
            var user = await _userApi.RegisterUser(new ExternalUserRequest
            {
                Id = userId,
                ConfirmPassword = request.ConfirmPassword,
                Email = request.Email,
                Password = request.Password,
                PhoneNumber = request.PhoneNumber,
                UserName = request.UserName
            });
            request.ExternalId = user.Id;
            var customerEntity = request.ToCustomerEntity();
            _customerRepository.Add(customerEntity);
            await _customerRepository.CommitAsync();
            await _auditEventLogger.LogEventAsync(new ApiCreateCustomerRequestEvent(request.Name, request.UserName,
                request.Email, request.PhoneNumber, request.Password, request.ConfirmPassword, request.Status,
                request.DeviceId, request.ExternalId));
            return Results.Ok();
        }

        public async Task<IResult> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            var item = await _customerRepository.GetByIdAsync(request.Id);
            if (item is null) throw new Exception($"Couldn't find entity with id={request.Id}");

            item.Update(request.Name);
            _customerRepository.Update(item);
            await _customerRepository.CommitAsync();
            await _auditEventLogger.LogEventAsync(new ApiUpdateCustomerRequestEvent(request.Id, request.Name));
            return Results.Ok();
        }
    }
}