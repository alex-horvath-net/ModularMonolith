namespace Accounts.Register;

internal record RequestBase(
    Guid CorrelationId,
    Guid RequestId);
