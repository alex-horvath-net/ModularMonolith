namespace Accounts.Login;

internal sealed record UserStoryRequest(
     Guid VisitorId,
     AccountType AccountType,
     IReadOnlyDictionary<string, string> Credentials);
