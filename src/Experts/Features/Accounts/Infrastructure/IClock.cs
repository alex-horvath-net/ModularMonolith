namespace Business.Features.Accounts.Infrastructure;

public interface IClock {
    DateTime UtcNow { get; }
}