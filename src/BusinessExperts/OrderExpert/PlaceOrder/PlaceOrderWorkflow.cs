using Experts.OrderExpert.PlaceOrder.Shared.Business.Domain;

namespace Experts.OrderExpert.PlaceOrder;

public class PlaceOrderWorkflow(
   Valide.Business validator,
   Create.Business factory,
   Store.Business store,
   Publish.Business publisher) {

    public async Task<CreateOrderResponse> Run(CreateOrderRequest request, CancellationToken token) {        

        var response = new CreateOrderResponse();

        response.Errors = await validator.Validate(request, token);
        if (response.Errors.Any()) {
            return response;
        }

        response.Order = factory.Create(request);

        await store.Save(response.Order, token);

        response.IsOrderPlaced = await publisher.Publish(response.Order, token);

        return response;
    }
}
