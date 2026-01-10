using Experts.OrderExpert.PlaceOrderFlow.Shared.Business.Domain;

namespace Experts.OrderExpert.PlaceOrderFlow;

public class PlaceOrderWorkflow(
   ValideStep.Business validate,
   CreateStep.Business create,
   SaveStep.Business save,
   PublishStep.Business publish) {

    public async Task<CreateOrderResponse> Run(CreateOrderRequest request, CancellationToken token) {

        var response = new CreateOrderResponse();

        response.Errors = await validate.Run(request, token);
        if (response.Errors.Any()) {
            return response;
        }

        response.Order = create.Run(request);

        await save.Run(response.Order, token);

        await publish.Run(response.Order, token);

        return response;
    }
}
