using Common.Events;

namespace Business.Experts.Shared.Business.Events;

public sealed record OrderPlaced(
    Guid OrderId, 
    Guid CustomerId, 
    decimal Total) : IBusinessEvent;
