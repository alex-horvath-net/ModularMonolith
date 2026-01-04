using Common.Events;

namespace Business.ApplicationUsers.Member.Contracts.Events;

public sealed record OrderPlaced(Guid OrderId, Guid CustomerId, decimal Total) : IBusinessEvent;
