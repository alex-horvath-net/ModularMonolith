namespace Experts.OrderExpert.PlaceOrder.Shared.Business.Domain;

public sealed record CreateOrderRequest(
    Guid CustomerId, 
    IEnumerable<CreateOrderLineRequest> Lines);
