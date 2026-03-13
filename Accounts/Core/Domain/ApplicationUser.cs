namespace Accounts.Core.Domain;

public record ApplicationUser(
    Application Application,
    Identity Identity,
    IReadOnlyList<string> Roles);
