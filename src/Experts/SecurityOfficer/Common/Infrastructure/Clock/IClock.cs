namespace Experts.SecurityOfficer.Common.Infrastructure.Clock;

public interface IClock {
    DateTime UtcNow { get; }
}