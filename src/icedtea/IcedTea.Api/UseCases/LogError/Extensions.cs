using Microsoft.AspNetCore.Authorization;

namespace IcedTea.Api.UseCases.LogError;

public static class Extensions
{
    public static WebApplication UseLogErrorEndpoint(this WebApplication app)
    {
        app.MapGet("api/v1/admin/log-errors",
                [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
                async ([FromQuery] int skip, int take, string? query, ISender sender) =>
                    await sender.Send(new MutateLogError.GetListLogErrorQueries(skip, take, query)))
            .Produces(200, typeof(ResultModel<QueryResult<LogErrorDto>>))
            .WithTags("LogError")
            .ProducesProblem(404);

        app.MapDelete("api/v1/admin/log-errors",
                [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
                async ([FromBody] MutateLogError.DeleteLogErrorCommand command, ISender sender) =>
                    await sender.Send(command))
            .Produces(200, typeof(IResult))
            .WithTags("LogError")
            .ProducesProblem(404);

        return app;
    }
}