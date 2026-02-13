namespace Business.Modules.SecurityOfficer.Infrastructure.GuidNumber;

internal class GuidGenerator : IGuid {
    public Guid Generate() => Guid.NewGuid();
}
