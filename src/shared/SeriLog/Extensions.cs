using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Shared.SeriLog;

public static class Extensions
{
    public static async Task WithSeriLog(Func<ValueTask> func)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();
        Serilog.Debugging.SelfLog.Enable(Console.Error);
        Log.Information("Start Application");
        try
        {
            await func.Invoke();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Unhandled exception");
        }
        finally
        {
            Log.Information("Shut down complete");
            Log.CloseAndFlush();
        }
    }

    public static void AddSerilog(this ConfigureHostBuilder hostBuilder, string appName)
    {
        hostBuilder.UseSerilog((ctx, lc) => lc
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", appName)
            .ReadFrom.Configuration(ctx.Configuration));
    }
}