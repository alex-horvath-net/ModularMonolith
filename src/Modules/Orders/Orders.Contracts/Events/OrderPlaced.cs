using Common;

namespace Orders.Contracts.Events;

// Orders raises an event; Billing listens
public sealed record OrderPlaced(Guid OrderId, Guid CustomerId, decimal Total) : IBusinessEvent;

