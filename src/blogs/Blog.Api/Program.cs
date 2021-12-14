using Blog.Api;
using Microsoft.OpenApi.Models;
using BlogEntity = Blog.Domain.AggregatesModel.BlogAggregate.Blog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.AddSerilog("Blogs");
// Add services to the container.
builder.Services
    .AddCustomCors()
    .AddHttpContextAccessor()
    .AddCustomMediatR(new[] { typeof(Anchor) })
    .AddCustomValidators(new[] { typeof(Anchor) })
    .AddSwaggerGen(setup =>
    {
        setup.SwaggerDoc("v1", new OpenApiInfo()
            {
                Description = "Blog web api implementation using Minimal Api in Asp.Net Core",
                Title = "Blog Api",
                Version = "v1",
                Contact = new OpenApiContact()
                {
                    Name = "MRA",
                    Url = new Uri("http://netcoreexamples.com")
                }
            });})
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
app.UseSwagger();
app.UseSwaggerUI();
app.MapFallback(() => Results.Redirect("/swagger"));
app.UseController();
await WithSeriLog(() =>
{
    app.AutoInit().Run();
    return ValueTask.CompletedTask;
});