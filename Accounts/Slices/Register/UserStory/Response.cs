namespace Accounts.Slices.Register.UserStory;

internal sealed record Response(
    string? ErrorMessage,
    Guid AccountId,
    string? Email,
    string? UserName,
    IReadOnlyCollection<string> Roles);
