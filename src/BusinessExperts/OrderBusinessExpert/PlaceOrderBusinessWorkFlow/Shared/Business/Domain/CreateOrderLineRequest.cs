namespace Experts.OrderBusinessExpert.PlaceOrderBusinessWorkFlow.Shared.Business.Domain;

public record CreateOrderLineRequest(
    Guid ProductId, 
    int Quantity, 
    decimal UnitPrice);
