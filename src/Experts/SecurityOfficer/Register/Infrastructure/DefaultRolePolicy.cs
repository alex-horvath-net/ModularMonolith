using System;
using System.Collections.Immutable;
using System.Linq;

namespace Experts.SecurityOfficer.Register.Infrastructure;

public sealed class DefaultRolePolicy : UserStory.IRolePolicy
{
    private static readonly ImmutableHashSet<string> AllowedRoles =
        ImmutableHashSet.Create(StringComparer.OrdinalIgnoreCase, "Trader", "RiskManager", "Compliance");

    public bool AreEligible(IEnumerable<string> requestedRoles)
    {
        ArgumentNullException.ThrowIfNull(requestedRoles);
        return requestedRoles.All(role => AllowedRoles.Contains(role));
    }
}
