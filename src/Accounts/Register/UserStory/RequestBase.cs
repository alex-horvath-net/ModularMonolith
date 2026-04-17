namespace Accounts.Register.UserStory;

internal record RequestBase(
    Guid CorrelationId,
    Guid RequestId);
