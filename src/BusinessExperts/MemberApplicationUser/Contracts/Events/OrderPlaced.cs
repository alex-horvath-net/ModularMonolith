using Common.Events;

namespace Business.MemberApplicationUser.Contracts.Events;

public sealed record OrderPlaced(Guid OrderId, Guid CustomerId, decimal Total) : IBusinessEvent;
