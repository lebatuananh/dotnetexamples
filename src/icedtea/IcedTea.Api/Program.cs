using IcedTea.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Host.AddSerilog("Icedtea");
// Add services to the container.
builder.Services
    .AddCustomCors()
    .AddHttpContextAccessor()
    .AddCustomMediatR(new[] { typeof(Anchor) })
    .AddCustomValidators(new[] { typeof(Anchor) })
    .AddAuthenticationCustom(builder.Configuration)
    .AddAuthorizationPolicies(builder.Configuration)
    .AddSwaggerConfig(builder.Configuration)
    .AddPersistence(builder.Configuration)
    .AddRepository()
    .AddEndpointsApiExplorer()
    .AddInitializationStages()
    .AddControllers()
    .AddNewtonsoftJson();

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
await WithSeriLog(() =>
{
    app.AutoInit().Run();
    return ValueTask.CompletedTask;
});