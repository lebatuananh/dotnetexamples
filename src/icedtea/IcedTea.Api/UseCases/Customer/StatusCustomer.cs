using AuditLogging.Services;
using IcedTea.Domain.AggregateModel.CustomerAggregate;

namespace IcedTea.Api.UseCases.Customer;

public struct StatusCustomer
{
    public record EnableCustomerCommand : IUpdateCommand<Guid>
    {
        public Guid Id { get; init; }


        internal class Validator : AbstractValidator<EnableCustomerCommand>
        {
            public Validator()
            {
                RuleFor(x => x.Id)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Id is not empty");
            }
        }
    }

    public record DisableCustomerCommand : IUpdateCommand<Guid>
    {
        public Guid Id { get; init; }


        internal class Validator : AbstractValidator<DisableCustomerCommand>
        {
            public Validator()
            {
                RuleFor(x => x.Id)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Id is not empty");
            }
        }
    }

    internal class Handler :
        IRequestHandler<DisableCustomerCommand, IResult>,
        IRequestHandler<EnableCustomerCommand, IResult>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IAuditEventLogger _auditEventLogger;

        public Handler(ICustomerRepository customerRepository, IAuditEventLogger auditEventLogger)
        {
            _customerRepository = customerRepository;
            _auditEventLogger = auditEventLogger;
        }

        public async Task<IResult> Handle(DisableCustomerCommand request, CancellationToken cancellationToken)
        {
            var item = await _customerRepository.GetByIdAsync(request.Id);
            if (item is null) throw new Exception($"Couldn't find entity with id={request.Id}");

            item.Disable();
            _customerRepository.Update(item);
            await _customerRepository.CommitAsync();
            await _auditEventLogger.LogEventAsync(new ApiDisableCustomerRequestEvent(request.Id, item));
            return Results.Ok();
        }

        public async Task<IResult> Handle(EnableCustomerCommand request, CancellationToken cancellationToken)
        {
            var item = await _customerRepository.GetByIdAsync(request.Id);
            if (item is null) throw new Exception($"Couldn't find entity with id={request.Id}");

            item.Enable();
            _customerRepository.Update(item);
            await _customerRepository.CommitAsync();
            await _auditEventLogger.LogEventAsync(new ApiEnableCustomerRequestEvent(request.Id, item));
            return Results.Ok();
        }
    }
}