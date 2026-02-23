namespace Features.Accounts.Domain;

public record ApplicationUser(
    Application Application,
    Identity Identity,
    IReadOnlyList<string> Roles);
