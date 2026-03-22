namespace Accounts.Register.UserStory;

public sealed record Response(
    string? ErrorMessage,
    Guid AccountId,
    string? Email,
    string? UserName,
    IReadOnlyCollection<string> Roles);
