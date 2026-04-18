using Microsoft.Extensions.Logging;

namespace Core.Infrastructure.Logger;

internal class MicrosoftLogger<T>(ILoggerFactory loggerFactory) : ILogger<T> {

    private readonly Microsoft.Extensions.Logging.ILogger<T> logger = loggerFactory.CreateLogger<T>();
    public void LogDebug(string? messageTemplate, params object?[] args) {
        if (logger.IsEnabled(LogLevel.Debug))
            logger.LogDebug(messageTemplate, args);
    }

    public void LogInformation(string? messageTemplate, params object?[] args) {
        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation(messageTemplate, args);
    }

    public void LogWarning(string? messageTemplate, params object?[] args) {
        if (logger.IsEnabled(LogLevel.Warning))
            logger.LogWarning(messageTemplate, args);
    }

    public void LogWarning(Exception? exception, string? messageTemplate, params object?[] args) {
        if (logger.IsEnabled(LogLevel.Warning))
            logger.LogWarning(exception, messageTemplate, args);
    }

    public void LogError(string? messageTemplate, params object?[] args) {
        if (logger.IsEnabled(LogLevel.Error))
            logger.LogError(messageTemplate, args);
    }

    public void LogError(Exception? exception, string? messageTemplate, params object?[] args) {
        if (logger.IsEnabled(LogLevel.Error))
            logger.LogError(exception, messageTemplate, args);
    }

    public ILogger<K> As<K>() => new MicrosoftLogger<K>(loggerFactory);
}

public static class LoggerFactory {
    public static ILogger<T> Create<T>() => default;
}