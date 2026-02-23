namespace Core.Events;

public sealed record OrderPlaced(
    Guid OrderId,
    Guid CustomerId,
    decimal Total) : IBusinessEvent;
