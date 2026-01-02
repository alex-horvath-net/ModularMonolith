using Common.Events;

namespace BusinessExperts.Contracts.Events;

public sealed record OrderPlaced(Guid OrderId, Guid CustomerId, decimal Total) : IBusinessEvent;
