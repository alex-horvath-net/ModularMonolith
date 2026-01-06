using Business.OrderBusinessExpert.PlaceOrderBusinessWorkFlow.Domain;

namespace Business.OrderBusinessExpert.PlaceOrderBusinessWorkFlow.WorkSteps;

public class CreateOrderWorkStep {
    public Order Create(CreateOrderRequest request) => Order.Create(
        request.CustomerId, 
        request.Lines.Select(l => (l.ProductId, l.Quantity, l.UnitPrice)));
}
