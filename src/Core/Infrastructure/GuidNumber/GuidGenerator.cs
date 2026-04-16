namespace Core.Infrastructure.GuidNumber;

internal class GuidGenerator : IGuidGenerator {
    public Guid New() => Guid.NewGuid();
}
