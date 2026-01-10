using Business.Experts.OrderBusinessExpert.Shared.Business.Domain;
using Business.Experts.OrderBusinessExpert.WorkFlows.PlaceOrderBusinessWorkFlow.Shared.Business.Domain;

namespace Business.Experts.OrderBusinessExpert.WorkFlows.PlaceOrderBusinessWorkFlow.WorkSteps;

public class CreateOrderWorkStep {
    public Order Create(CreateOrderRequest request) {
        var order = new Order(
        request.CustomerId);

        foreach (var line in request.Lines) {
            order.AddLine(line.ProductId, line.Quantity, line.UnitPrice);
        }
        return order;
    }
}
