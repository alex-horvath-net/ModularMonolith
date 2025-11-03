using Common;

namespace Orders.Contracts;
public sealed record OrderPlaced(Guid OrderId, Guid CustomerId, decimal Total) : IEvent;

