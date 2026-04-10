namespace Accounts.Register.UserStory;

internal sealed record Request(
    Guid CorrelationId,
    string Email,
    string UserName,
    string Password,
    IReadOnlyCollection<string> Roles);
