using Blog.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Host.AddSerilog("Blogs");
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
    c.OAuthClientId("blog_admin_api_swaggerui");
    c.OAuthAppName("BlogApi");
    c.OAuthUsePkce();
});
app.MapFallback(() => Results.Redirect("/swagger"));
app.UseEndpoint();
await WithSeriLog(async () =>
{
    await app.AutoInit(app.Logger);
    app.Run();
});