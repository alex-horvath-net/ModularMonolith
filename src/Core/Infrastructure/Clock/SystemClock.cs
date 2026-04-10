namespace Core.Infrastructure.Clock;

public class SystemClock : IClock {
    public DateTime UtcNow => DateTimeOffset.UtcNow.UtcDateTime;
}
