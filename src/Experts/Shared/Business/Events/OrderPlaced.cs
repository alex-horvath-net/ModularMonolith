using Common.Events;

namespace BusinessExperts.Shared.Business.Events;

public sealed record OrderPlaced(
    Guid OrderId, 
    Guid CustomerId, 
    decimal Total) : IBusinessEvent;
