using Experts.OrderExpert.PlaceOrder.Shared.Business.Domain;

namespace Experts.OrderExpert.PlaceOrder;

public class PlaceOrderWorkflow(
   Validate.Business validate,
   Create.Business create,
   Save.Business save,
   Publish.Business publish) {

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
