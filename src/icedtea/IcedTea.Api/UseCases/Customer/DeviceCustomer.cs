using AuditLogging.Services;
using IcedTea.Domain.AggregateModel.CustomerAggregate;

namespace IcedTea.Api.UseCases.Customer;

public struct DeviceCustomer
{
    public record UpdateDeviceCustomerCommand(Guid Id) : IUpdateCommand<Guid>
    {
        public string DeviceId { get; init; }

        internal class Validator : AbstractValidator<UpdateDeviceCustomerCommand>
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

    internal class Handler : IRequestHandler<UpdateDeviceCustomerCommand, IResult>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IAuditEventLogger _auditEventLogger;

        public Handler(ICustomerRepository customerRepository, IAuditEventLogger auditEventLogger)
        {
            _customerRepository = customerRepository;
            _auditEventLogger = auditEventLogger;
        }

        public async Task<IResult> Handle(UpdateDeviceCustomerCommand request, CancellationToken cancellationToken)
        {
            var item = await _customerRepository.GetByIdAsync(request.Id);
            if (item is null) throw new Exception($"Couldn't find entity with id={request.Id}");

            item.UpdateDeviceId(request.DeviceId);
            _customerRepository.Update(item);
            await _customerRepository.CommitAsync();
            await _auditEventLogger.LogEventAsync(
                new ApiUpdateDeviceCustomerRequestEvent(request.Id, request.DeviceId));
            return Results.Ok();
        }
    }
}