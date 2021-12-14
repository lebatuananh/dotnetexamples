using Blog.Api;
using BlogEntity = Blog.Domain.AggregatesModel.BlogAggregate.Blog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.AddSerilog("Blogs");
// Add services to the container.
builder.Services
    .AddCustomCors()
    .AddHttpContextAccessor()
    .AddCustomValidators(new[] { typeof(BlogEntity), typeof(BlogTag), typeof(Tag) })
    .AddCustomValidators(new[] { typeof(BlogEntity), typeof(BlogTag), typeof(Tag) })
    .AddSwaggerGen()
    .AddPersistence(builder.Configuration)
    .AddRepository()
    .AddEndpointsApiExplorer()
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