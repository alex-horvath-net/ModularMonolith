namespace Experts.Experts.OrderBusinessExpert.WorkFlows.PlaceOrderBusinessWorkFlow.Domain;

public sealed record CreateOrderRequest(Guid CustomerId, IEnumerable<CreateOrderLineRequest> Lines);
