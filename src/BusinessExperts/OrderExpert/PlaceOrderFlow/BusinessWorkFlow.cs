using Experts.OrderExpert.PlaceOrderFlow.Shared.Business.Domain;
using Experts.OrderExpert.Shared.Business.Domain;
using Experts.Shared.Business.Domain;

namespace Experts.OrderExpert.PlaceOrderFlow;

public class BusinessWorkFlow(BusinessWorkFlow.IBusinessWorkSteps workSteps) {
    public async Task<CreateOrderResponse> Run(CreateOrderRequest request, CancellationToken token) {

        var response = new CreateOrderResponse();

        response.Errors = await workSteps.Validate(request, token);
        if (response.Errors.Any())
            return response;

        response.Order = workSteps.Create(request);

        await workSteps.Save(response.Order, token);

        await workSteps.Publish(response.Order, token);

        return response;
    }

    public interface IBusinessWorkSteps {
        Task<IEnumerable<Error>> Validate(CreateOrderRequest request, CancellationToken token);
        Order Create(CreateOrderRequest request);
        Task Save(Order order, CancellationToken token);
        Task Publish(Order order, CancellationToken token);
    }
}
