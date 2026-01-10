using Experts.Order.PlaceOrder.Shared.Business.Domain;

namespace Experts.Order.PlaceOrder;

public class PlaceOrderWorkflow(
   ValidatorBusinessWorkSteps.Business validator,
   FactoryBusinessWorkSteps.Business factory,
   StoreBusinessWorkSteps.Business store,
   PublisherBusinessWorkStep.Business publisher) {

    public async Task<CreateOrderResponse> Run(CreateOrderRequest request, CancellationToken token) {

        var response = new CreateOrderResponse();

        response.Errors = await validator.Validate(request, token);
        if (response.Errors.Any()) {
            return response;
        }

        response.Order = factory.Create(request);
        response.Order = factory.Create(request);

        await store.Save(response.Order, token);

        response.IsOrderPlaced = await publisher.Publish(response.Order, token);

        return response;
    }
}
