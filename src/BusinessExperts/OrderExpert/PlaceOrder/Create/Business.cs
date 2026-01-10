using Experts.OrderExpert.PlaceOrder.Shared.Business.Domain;
using Experts.OrderExpert.Shared.Business.Domain;

namespace Experts.OrderExpert.PlaceOrder.Create;

public class Business() {
    public Order Create(CreateOrderRequest request) {
        var order = new Order(request.CustomerId);

        foreach (var line in request.Lines) {
            order.AddLine(line.ProductId, line.Quantity, line.UnitPrice);
        }

        return order;
    }
}
