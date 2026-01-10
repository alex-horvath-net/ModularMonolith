using Experts.Order.PlaceOrderBusinessWorkFlow.Shared.Business.Domain;

namespace Experts.Order.PlaceOrderBusinessWorkFlow.FactoryBusinessWorkSteps;

public class Business() {
    public Order.Shared.Business.Domain.Order Create(CreateOrderRequest request) {
        var order = new Order.Shared.Business.Domain.Order(request.CustomerId);

        foreach (var line in request.Lines) {
            order.AddLine(line.ProductId, line.Quantity, line.UnitPrice);
        }

        return order;
    }
}
