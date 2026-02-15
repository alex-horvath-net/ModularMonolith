namespace Business.Experts.SecurityOfficer.Infrastructure.Clock;

public interface IClock {
    DateTime UtcNow { get; }
}