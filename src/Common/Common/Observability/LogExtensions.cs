using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Events;

namespace Common.Observability;

public static class LogExtensions {
    public static ConfigureHostBuilder UseLogger(this ConfigureHostBuilder host) {
        // Structured logging configuration
        host.UseSerilog((ctx, cfg) => cfg
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.WithProperty("Application", "WebApi")
            .Enrich.WithProperty("Environment", ctx.HostingEnvironment.EnvironmentName)
            .Enrich.WithProperty("MachineName", Environment.MachineName)
            .Enrich.FromLogContext()
            .WriteTo.Console(formatProvider: System.Globalization.CultureInfo.InvariantCulture)
            .WriteTo.Debug());

        return host;
    }
}     