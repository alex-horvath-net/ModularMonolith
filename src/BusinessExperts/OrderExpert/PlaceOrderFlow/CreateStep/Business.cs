using Experts.OrderExpert.PlaceOrderFlow.Shared.Business.Domain;
using Experts.OrderExpert.Shared.Business.Domain;

namespace Experts.OrderExpert.PlaceOrderFlow.CreateStep;

public class Business() {
    public Order Run(CreateOrderRequest request) {
        var order = new Order(request.CustomerId);

        foreach (var line in request.Lines) {
            order.AddLine(line.ProductId, line.Quantity, line.UnitPrice);
        }

        return order;
    }
}
