namespace Experts.SecurityOfficer.Common.Domain;

public record ApplicationUser(
    Application Application,
    Identity Identity,
    IReadOnlyList<string> Roles);
