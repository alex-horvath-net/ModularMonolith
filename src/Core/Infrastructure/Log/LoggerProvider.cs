using Microsoft.Extensions.Logging;

namespace Core.Infrastructure.Log;

public static class LoggerProvider {
    public static ILoggerFactory Factory { get; set; } = null!;

    public static ILogger<T> Create<T>(ILoggerFactory? factory = null) => new Logger<T>(factory ?? Factory);
}