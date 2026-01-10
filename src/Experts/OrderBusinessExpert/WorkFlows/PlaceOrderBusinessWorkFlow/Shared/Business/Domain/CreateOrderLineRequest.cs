namespace Business.Experts.OrderBusinessExpert.WorkFlows.PlaceOrderBusinessWorkFlow.Shared.Business.Domain;

public record CreateOrderLineRequest(Guid ProductId, int Quantity, decimal UnitPrice);
