namespace Accounts.Register.UserStory;

internal sealed record Request(
    string Email,
    string UserName,
    string Password,
    IReadOnlyCollection<string> Roles,
    Guid CorrelationId,
    Guid RequestId) : RequestBase(CorrelationId, RequestId);
