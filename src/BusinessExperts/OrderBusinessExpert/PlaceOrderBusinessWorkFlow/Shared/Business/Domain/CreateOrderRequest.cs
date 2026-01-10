namespace Experts.OrderBusinessExpert.PlaceOrderBusinessWorkFlow.Shared.Business.Domain;

public sealed record CreateOrderRequest(
    Guid CustomerId, 
    IEnumerable<CreateOrderLineRequest> Lines);
