namespace Experts.SecurityOfficer.Common.Infrastructure.Clock;

public class SystemClock : IClock {
    public DateTime UtcNow => DateTime.UtcNow;
}
