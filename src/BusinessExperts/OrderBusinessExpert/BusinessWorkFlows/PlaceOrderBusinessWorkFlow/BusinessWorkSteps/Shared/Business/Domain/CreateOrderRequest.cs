namespace BusinessExperts.OrderBusinessExpert.BusinessWorkFlows.PlaceOrderBusinessWorkFlow.BusinessWorkSteps.Shared.Business.Domain;

public sealed record CreateOrderRequest(
    Guid CustomerId, 
    IEnumerable<CreateOrderLineRequest> Lines);
