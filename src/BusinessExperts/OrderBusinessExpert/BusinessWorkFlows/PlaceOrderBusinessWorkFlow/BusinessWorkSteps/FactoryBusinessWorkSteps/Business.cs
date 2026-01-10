using BusinessExperts.OrderBusinessExpert.BusinessWorkFlows.PlaceOrderBusinessWorkFlow.BusinessWorkSteps.Shared.Business.Domain;
using BusinessExperts.OrderBusinessExpert.BusinessWorkFlows.Shared.Business.Domain;

namespace BusinessExperts.OrderBusinessExpert.BusinessWorkFlows.PlaceOrderBusinessWorkFlow.BusinessWorkSteps.FactoryBusinessWorkSteps;

public class Business() {
    public Order Create(CreateOrderRequest request) {
        var order = new Order(request.CustomerId);

        foreach (var line in request.Lines) {
            order.AddLine(line.ProductId, line.Quantity, line.UnitPrice);
        }

        return order;
    }
}
