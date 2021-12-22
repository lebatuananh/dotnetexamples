using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;

namespace IcedTea.Api.UseCases.Customer;

public static class Extensions
{
    public static WebApplication UseCustomerEndpoint(this WebApplication app)
    {
        app.MapGet("api/v1/admin/customer",
                [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
                async ([FromQuery] int skip, int take, string? query, ISender sender) =>
                    await sender.Send(new MutateCustomer.GetListCustomerQueries(skip, take, query)))
            .Produces(200, typeof(ResultModel<QueryResult<CustomerDto>>))
            .WithTags("Customer")
            .ProducesProblem(404);

        app.MapPost("api/v1/admin/customer",
                [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
                async ([FromBody] MutateCustomer.CreateCustomerCommand command, ISender sender) =>
                    await sender.Send(command)).Produces(200)
            .WithTags("Customer")
            .ProducesProblem(401);

        app.MapPut("api/v1/admin/customer/{id}",
                [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
                async ([FromRoute] Guid id, MutateCustomer.UpdateCustomerCommand command, ISender sender) =>
                    await sender.Send(command with { Id = id }))
            .Produces(200)
            .WithTags("Customer");

        app.MapPut("api/v1/admin/customer/{id}/device",
                [Authorize] async ([FromRoute] Guid id, DeviceCustomer.UpdateDeviceCustomerCommand command,
                        ISender sender) =>
                    await sender.Send(command with { Id = id }))
            .Produces(200)
            .WithTags("Customer");


        app.MapPut("api/v1/admin/customer/{id}/enable",
                [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
                async ([FromRoute] Guid id, StatusCustomer.EnableCustomerCommand command,
                        ISender sender) =>
                    await sender.Send(command with { Id = id }))
            .Produces(200)
            .WithTags("Customer");


        app.MapPut("api/v1/admin/customer/{id}/disable",
                [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
                async ([FromRoute] Guid id, StatusCustomer.DisableCustomerCommand command,
                        ISender sender) =>
                    await sender.Send(command with { Id = id }))
            .Produces(200)
            .WithTags("Customer");

        app.MapPost("api/v1/admin/customer/{id}/deposit",
                [Authorize] async ([FromRoute] Guid id, TransactionCustomer.CustomerDepositCommand command,
                        ISender sender) =>
                    await sender.Send(command with
                    {
                        Id = id
                    }))
            .Produces(200)
            .WithTags("Customer");

        app.MapPut("api/v1/admin/customer/{id}/{transactionId}/deposit",
                [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
                async ([FromRoute] Guid id, [FromRoute] Guid transactionId,
                        TransactionCustomer.AcceptTransactionCustomerDepositCommand command,
                        ISender sender) =>
                    await sender.Send(command with
                    {
                        Id = id,
                        TransactionId = transactionId
                    }))
            .Produces(200)
            .WithTags("Customer");


        app.MapPut("api/v1/admin/customer/{id}/{cashFundId}/deposit-cash-fund",
                [Authorize]
                async ([FromRoute] Guid id, [FromRoute] Guid cashFundId,
                        TransactionCustomer.CustomerDepositCashFundCommand command,
                        ISender sender) =>
                    await sender.Send(command with
                    {
                        Id = id,
                        CashFundId = cashFundId
                    }))
            .Produces(200)
            .WithTags("Customer");


        app.MapGet("api/v1/admin/customer/{id}",
                [Authorize] async ([FromRoute] Guid id, ISender sender) =>
                    await sender.Send(new MutateCustomer.GetCustomerQuery() { Id = id }))
            .Produces(200, typeof(ResultModel<CustomerDto>))
            .WithTags("Customer").ProducesProblem(404);
        return app;
    }
}