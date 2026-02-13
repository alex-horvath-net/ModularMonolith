namespace Experts.SecurityOfficer.Common.Infrastructure.GuidNumber;

internal class GuidGenerator : IGuid {
    public Guid New() => Guid.NewGuid();
}
