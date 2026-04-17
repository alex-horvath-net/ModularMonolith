namespace Accounts.Register.BlazorTrigger;

public sealed record RegisterBlazorResponse(
    Guid AccountId,
    string Email,
    IReadOnlyCollection<string> Roles,
    string? ErrorMessage);
