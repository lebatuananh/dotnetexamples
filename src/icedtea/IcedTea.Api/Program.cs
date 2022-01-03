using AuditLogging.EntityFramework.Entities;
using IcedTea.Api;
using Shared.Audit;

await WithSeriLog(async () =>
{
    var builder = WebApplication.CreateBuilder(args);
    var configuration = builder.Configuration;
    configuration.AddJsonFile("user.endpoint.json", optional: true, reloadOnChange: true);
    configuration.AddJsonFile("serilog.json", optional: true, reloadOnChange: true);
    configuration.AddEnvironmentVariables();
    configuration.AddCommandLine(args);
    builder.Host.AddSerilog("Icedtea");
    // Add services to the container.
    builder.Services
        .AddCustomCors()
        .AddHttpContextAccessor()
        .AddHttpClient(builder.Configuration)
        .AddCustomMediatR(new[] { typeof(Anchor) })
        .AddCustomValidators(new[] { typeof(Anchor) })
        .AddApiClient()
        .AddAuthenticationCustom(builder.Configuration)
        .AddAuthorizationPolicies(builder.Configuration)
        .AddSwaggerConfig(builder.Configuration)
        .AddPersistence(builder.Configuration)
        .AddRepository()
        .AddConfig(builder.Configuration)
        .AddAuditEventLogging<AuditLogDbContext, AuditLog>(builder.Configuration)
        .AddEndpointsApiExplorer()
        .AddInitializationStages()
        .AddControllers();
    var app = builder.Build();
    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment()) app.UseExceptionHandler("/Error");
    app.MapGet("/error", () => Results.Problem("An error occurred.", statusCode: 500))
        .ExcludeFromDescription();

    app.UseMiddleware<ExceptionMiddleware>();
    app.UseCustomCors();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.OAuthClientId("iced_tea_api_swaggerui");
        c.OAuthAppName("IcedTeaApi");
        c.OAuthUsePkce();
    });
    app.MapFallback(() => Results.Redirect("/swagger"));
    app.UseEndpoint();
    await app.AutoInit(app.Logger);
    app.Run();
});