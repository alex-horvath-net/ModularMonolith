namespace Business.Modules.SecurityOfficer.Infrastructure.Clock;

public interface IClock {
    DateTime UtcNow { get; }
}