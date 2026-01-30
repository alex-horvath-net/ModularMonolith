namespace Experts.SecurityOfficer.Shared.Domain;

public record ApplicationUser(
    Application Application,
    Identity Identity,
    IReadOnlyList<string> Roles);
