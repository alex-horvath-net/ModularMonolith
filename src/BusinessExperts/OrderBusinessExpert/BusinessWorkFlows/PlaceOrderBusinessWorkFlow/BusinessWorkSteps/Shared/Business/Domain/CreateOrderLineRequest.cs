namespace Experts.OrderBusinessExpert.BusinessWorkFlows.PlaceOrderBusinessWorkFlow.BusinessWorkSteps.Shared.Business.Domain;

public record CreateOrderLineRequest(
    Guid ProductId, 
    int Quantity, 
    decimal UnitPrice);
