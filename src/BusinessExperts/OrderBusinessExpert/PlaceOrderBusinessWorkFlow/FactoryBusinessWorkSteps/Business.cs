using Experts.OrderBusinessExpert.PlaceOrderBusinessWorkFlow.Shared.Business.Domain;
using Experts.OrderBusinessExpert.Shared.Business.Domain;

namespace Experts.OrderBusinessExpert.PlaceOrderBusinessWorkFlow.FactoryBusinessWorkSteps;

public class Business() {
    public Order Create(CreateOrderRequest request) {
        var order = new Order(request.CustomerId);

        foreach (var line in request.Lines) {
            order.AddLine(line.ProductId, line.Quantity, line.UnitPrice);
        }

        return order;
    }
}
