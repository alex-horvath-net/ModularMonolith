namespace Business.Features.Accounts.Infrastructure.GuidNumber;

internal class GuidGenerator : IGuid {
    public Guid Generate() => Guid.NewGuid();
}
