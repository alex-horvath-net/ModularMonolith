using Microsoft.Extensions.Logging;

namespace Core.Infrastructure.Log;

public static class LoggerFactory {
    public static ILoggerFactory Factory { get; set; } = Microsoft.Extensions.Logging.LoggerFactory.Create(builder => builder
    .SetMinimumLevel(LogLevel.Information)
    //.AddFilter("Microsoft", LogLevel.Warning)
    .AddConsole()
    .AddDebug());

    public static ILogger<T> Create<T>(ILoggerFactory? factory = null) => new Logger<T>(factory ?? Factory);
}