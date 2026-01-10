namespace Experts.Order.PlaceOrderBusinessWorkFlow.Shared.Business.Domain;

public record CreateOrderLineRequest(
    Guid ProductId, 
    int Quantity, 
    decimal UnitPrice);
