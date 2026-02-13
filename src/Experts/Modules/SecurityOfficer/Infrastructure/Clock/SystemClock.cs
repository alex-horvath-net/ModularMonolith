namespace Business.Modules.SecurityOfficer.Infrastructure.Clock;

public class SystemClock : IClock {
    public DateTime UtcNow => DateTime.UtcNow;
}
