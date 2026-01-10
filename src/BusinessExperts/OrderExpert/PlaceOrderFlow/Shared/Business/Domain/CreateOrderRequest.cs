namespace Experts.OrderExpert.PlaceOrderFlow.Shared.Business.Domain;

public sealed record CreateOrderRequest(
    Guid CustomerId, 
    IEnumerable<CreateOrderLineRequest> Lines);
