using AuditLogging.Services;
using IcedTea.Domain.AggregateModel.CustomerAggregate;
using User.Api;
using User.Api.Models;

namespace IcedTea.Api.UseCases.Customer;

public struct InformationCustomer
{
    public record ChangePasswordCommand : ICreateCommand
    {
        public string Password { get; init; }
        public string ConfirmPassword { get; init; }

        internal class Validator : AbstractValidator<ChangePasswordCommand>
        {
            public Validator()
            {
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

    public record ChangePasswordUserCommand(Guid Id) : IUpdateCommand<Guid>
    {
        public string Password { get; init; }
        public string ConfirmPassword { get; init; }

        internal class Validator : AbstractValidator<ChangePasswordUserCommand>
        {
            public Validator()
            {
                RuleFor(x => x.Id)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Id is not empty");
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


    public record AssignRoleCommand(Guid Id) : IUpdateCommand<Guid>
    {
        public Guid RoleId { get; init; }

        internal class Validator : AbstractValidator<AssignRoleCommand>
        {
            public Validator()
            {
                RuleFor(x => x.Id)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Id is not empty");

                RuleFor(x => x.RoleId)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("RoleId is not empty");
            }
        }
    }

    internal class Handler :
        IRequestHandler<ChangePasswordCommand, IResult>,
        IRequestHandler<ChangePasswordUserCommand, IResult>,
        IRequestHandler<AssignRoleCommand, IResult>
    {
        private readonly IUserApi _userApi;
        private readonly ICustomerRepository _customerRepository;
        private readonly IScopeContext _scopeContext;
        private readonly IAuditEventLogger _auditEventLogger;

        public Handler(IUserApi userApi, ICustomerRepository customerRepository, IScopeContext scopeContext,
            IAuditEventLogger auditEventLogger)
        {
            _userApi = userApi;
            _customerRepository = customerRepository;
            _scopeContext = scopeContext;
            _auditEventLogger = auditEventLogger;
        }

        public async Task<IResult> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var item = await _customerRepository.GetSingleAsync(
                x => x.ExternalId.Equals(_scopeContext.CurrentAccountId));
            if (item is null)
                throw new Exception($"Couldn't find entity with externalId={_scopeContext.CurrentAccountId}");
            await _userApi.ChangePassword(new ChangePasswordRequest
            {
                Password = request.Password,
                ConfirmPassword = request.ConfirmPassword
            });
            await _auditEventLogger.LogEventAsync(
                new ApiChangePasswordRequestEvent(request.Password, request.ConfirmPassword));
            return Results.Ok();
        }

        public async Task<IResult> Handle(ChangePasswordUserCommand request, CancellationToken cancellationToken)
        {
            var item = await _customerRepository.GetByIdAsync(request.Id);
            if (item is null) throw new Exception($"Couldn't find entity with id={request.Id}");
            await _userApi.ChangePasswordUser(new ChangePasswordUserRequest
            {
                Password = request.Password,
                ConfirmPassword = request.ConfirmPassword,
                UserId = item.ExternalId
            });
            await _auditEventLogger.LogEventAsync(
                new ApiChangePasswordUserRequestEvent(request.Password, request.ConfirmPassword, request.Id));
            return Results.Ok();
        }

        public async Task<IResult> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
        {
            var item = await _customerRepository.GetByIdAsync(request.Id);
            if (item is null) throw new Exception($"Couldn't find entity with id={request.Id}");
            await _userApi.AssignRole(new AssignRoleRequest
            {
                RoleId = request.RoleId,
                UserId = item.ExternalId
            });
            await _auditEventLogger.LogEventAsync(new ApiAssignRoleRequestEvent(request.Id, request.RoleId));
            return Results.Ok();
        }
    }
}