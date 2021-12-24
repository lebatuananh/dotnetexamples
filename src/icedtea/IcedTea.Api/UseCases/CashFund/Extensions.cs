using IcedTea.Api.UseCases.Customer;
using Microsoft.AspNetCore.Authorization;

namespace IcedTea.Api.UseCases.CashFund;

public static class Extensions
{
    public static WebApplication UseCashFundEndpoint(this WebApplication app)
    {
        app.MapGet("api/v1/admin/cash-fund",
                [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
                async ([FromQuery] int skip, int take, string? query, ISender sender) =>
                    await sender.Send(new MutateCashFund.GetListCashFundQueries(skip, take, query)))
            .Produces(200, typeof(ResultModel<QueryResult<CashFundDto>>))
            .WithTags("CashFund")
            .ProducesProblem(404);

        app.MapPost("api/v1/admin/cash-fund",
                [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
                async ([FromBody] MutateCashFund.CreateCashFundCommand command, ISender sender) =>
                    await sender.Send(command)).Produces(200)
            .WithTags("CashFund")
            .ProducesProblem(401);

        app.MapPut("api/v1/admin/cash-fund/{id}",
                [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
                async ([FromRoute] Guid id, MutateCashFund.UpdateCashFundCommand command, ISender sender) =>
                    await sender.Send(command with { Id = id }))
            .Produces(200)
            .WithTags("CashFund");

        app.MapPut("api/v1/admin/cash-fund/{id}/charge-cash-fund",
                [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
                async ([FromRoute] Guid id, TransactionCashFund.ChargeCashFundCommand command, ISender sender) =>
                    await sender.Send(command with { Id = id }))
            .Produces(200)
            .WithTags("CashFund");

        app.MapGet("api/v1/admin/cash-fund/{id}",
                [Authorize] async ([FromRoute] Guid id, ISender sender) =>
                    await sender.Send(new MutateCashFund.GetCashFundQuery() { Id = id }))
            .Produces(200, typeof(ResultModel<CashFundDto>))
            .WithTags("CashFund").ProducesProblem(404);
        return app;
    }
}