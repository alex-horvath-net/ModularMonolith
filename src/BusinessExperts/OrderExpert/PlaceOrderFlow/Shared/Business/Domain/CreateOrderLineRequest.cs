namespace Experts.OrderExpert.PlaceOrderFlow.Shared.Business.Domain;

public record CreateOrderLineRequest(
    Guid ProductId, 
    int Quantity, 
    decimal UnitPrice);
