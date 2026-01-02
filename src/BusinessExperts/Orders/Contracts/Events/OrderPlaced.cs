using Common.Events;

namespace BusinessExperts.Orders.Contracts.Events;

public sealed record OrderPlaced(Guid OrderId, Guid CustomerId, decimal Total) : IBusinessEvent;
