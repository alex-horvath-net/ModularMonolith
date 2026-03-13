namespace Accounts.Slices.Register.Triggers.Blazor;

public sealed record RegisterResponse(
    Guid AccountId,
    string Email,
    IReadOnlyCollection<string> Roles,
    string? ErrorMessage);
