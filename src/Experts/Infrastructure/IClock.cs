namespace Business.Infrastructure;

public interface IClock {
    DateTime UtcNow { get; }
}