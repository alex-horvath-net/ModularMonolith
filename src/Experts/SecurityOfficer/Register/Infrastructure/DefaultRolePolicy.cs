using System.Collections.Immutable;

namespace Experts.SecurityOfficer.Register.Infrastructure;

public sealed class DefaultRolePolicy {
    private static readonly ImmutableHashSet<string> AllowedRoles =
        ImmutableHashSet.Create(StringComparer.OrdinalIgnoreCase,
            "Trader",
            "RiskManager",
            "Compliance");

    public bool AreEligible(IEnumerable<string> requestedRoles) {
        ArgumentNullException.ThrowIfNull(requestedRoles);
        return requestedRoles.All(AllowedRoles.Contains);
    }
}
