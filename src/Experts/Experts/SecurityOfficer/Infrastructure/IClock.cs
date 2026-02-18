namespace Business.Experts.SecurityOfficer.Infrastructure;

public interface IClock {
    DateTime UtcNow { get; }
}