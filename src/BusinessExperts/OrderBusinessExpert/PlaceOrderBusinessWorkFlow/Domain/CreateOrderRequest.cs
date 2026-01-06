namespace Business.OrderBusinessExpert.PlaceOrderBusinessWorkFlow.Domain;

public sealed record CreateOrderRequest(Guid CustomerId, IEnumerable<CreateOrderLineRequest> Lines);
