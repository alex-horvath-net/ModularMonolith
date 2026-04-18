using Microsoft.Extensions.Logging;

namespace Core.Infrastructure.Log;

public static class LoggerProvider {
    private static ILoggerFactory factory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder => { });

    public static ILoggerFactory Factory {
        get => factory;
        set {
            ArgumentNullException.ThrowIfNull(value);
            factory = value;
        }
    }

    public static ILogger<T> Create<T>(ILoggerFactory? loggerFactory = null) => new Logger<T>(loggerFactory ?? factory);
}