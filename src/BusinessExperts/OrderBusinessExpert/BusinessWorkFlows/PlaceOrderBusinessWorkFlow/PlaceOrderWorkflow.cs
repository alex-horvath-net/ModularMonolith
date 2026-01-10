using Experts.OrderBusinessExpert.BusinessWorkFlows.PlaceOrderBusinessWorkFlow.BusinessWorkSteps.Shared.Business.Domain;

namespace Experts.OrderBusinessExpert.BusinessWorkFlows.PlaceOrderBusinessWorkFlow;

public class PlaceOrderWorkflow(
   BusinessWorkSteps.ValidatorBusinessWorkSteps.Business validator,
   BusinessWorkSteps.FactoryBusinessWorkSteps.Business factory,
   BusinessWorkSteps.StoreBusinessWorkSteps.Business store,
   BusinessWorkSteps.PublisherBusinessWorkStep.Business publisher) {

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
