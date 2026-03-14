
namespace Core.Infrastructure;

public interface ILogger<T> {
    void LogDebug(string? messageTemplate, params object?[] args);
    void LogInformation(string? messageTemplate, params object?[] args);
    void LogWarning(string? messageTemplate, params object?[] args);
    void LogWarning(Exception? exception, string? messageTemplate, params object?[] args);
    void LogError(string? messageTemplate, params object?[] args);
    void LogError(Exception? exception, string? messageTemplate, params object?[] args);
}
