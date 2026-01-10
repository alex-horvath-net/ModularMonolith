namespace Experts.OrderExpert.PlaceOrder.Shared.Business.Domain;

public record CreateOrderLineRequest(
    Guid ProductId, 
    int Quantity, 
    decimal UnitPrice);
