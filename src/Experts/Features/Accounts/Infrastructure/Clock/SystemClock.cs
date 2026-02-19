namespace Business.Features.Accounts.Infrastructure.Clock;

public class SystemClock : IClock {
    public DateTime UtcNow => DateTime.UtcNow;
}
