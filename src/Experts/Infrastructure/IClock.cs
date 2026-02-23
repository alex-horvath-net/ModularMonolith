namespace Core.Infrastructure;

public interface IClock {
    DateTime UtcNow { get; }
}