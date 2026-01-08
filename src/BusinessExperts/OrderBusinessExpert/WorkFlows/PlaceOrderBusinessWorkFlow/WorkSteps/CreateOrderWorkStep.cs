using Experts.OrderBusinessExpert.WorkFlows.PlaceOrderBusinessWorkFlow.Domain;

namespace Experts.OrderBusinessExpert.WorkFlows.PlaceOrderBusinessWorkFlow.WorkSteps;

public class CreateOrderWorkStep {
    public Order Create(CreateOrderRequest request) => Order.Create(
        request.CustomerId, 
        request.Lines.Select(l => (l.ProductId, l.Quantity, l.UnitPrice)));
}
