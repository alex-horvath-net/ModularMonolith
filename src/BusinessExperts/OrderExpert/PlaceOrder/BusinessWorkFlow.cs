using Experts.OrderExpert.Shared.Business.Domain;
using Experts.Shared.Business.Domain;

namespace Experts.OrderExpert.PlaceOrder;

internal class BusinessWorkFlow(BusinessWorkFlow.IBusinessWorkSteps workSteps) {
    public async Task<PlaceOrderResponse> Run(PlaceOrderRequest request, CancellationToken token) {

        var response = new PlaceOrderResponse();

        response.Errors = await workSteps.Validate(request, token);
        if (response.Errors.Any())
            return response;

        response.Order = workSteps.Create(request);

        await workSteps.Save(response.Order, token);

        await workSteps.Publish(response.Order, token);

        return response;
    }

    public interface IBusinessWorkSteps {
        Task<IEnumerable<Error>> Validate(PlaceOrderRequest request, CancellationToken token);
        Order Create(PlaceOrderRequest request);
        Task Save(Order order, CancellationToken token);
        Task Publish(Order order, CancellationToken token);
    }
}


public sealed record PlaceOrderRequest(
    Guid CustomerId,
    IEnumerable<PlaceOrderLineRequest> Lines);
 

public record PlaceOrderLineRequest(
    Guid ProductId,
    int Quantity,
    decimal UnitPrice);

public sealed class PlaceOrderResponse {
    public Order? Order { get; set; }
    public IEnumerable<Error> Errors { get; set; } = [];
}
