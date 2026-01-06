using ApplicationUsers.Member.BusinessExperts.OrderBusinessExpert.CreateOrderWorkFlow.Infrastructure.Data.Models;

namespace BusinessExperts.Order.CreateOrderWorkFlow;

public class CreateOrderWorkFlow(
    ValidatorWorkStep validatorWorkStep,
    CreateOrderWorkStep createOrderWorkStep,
    PersistWorkStep persistWorkStep,
    PublishWorkStep publishWorkStep) {

    public async Task<CreateOrderResponse> Run(CreateOrderRequest request, CancellationToken token) {

        var response = new CreateOrderResponse();

        response.Errors = await validatorWorkStep.Validate(request, token);
        if (response.Errors.Any()) {
            return response;
        }

        response.Order = createOrderWorkStep.Create(request);

        await persistWorkStep.Save(response.Order, token);
       
        response.IsOrderPlaced = await publishWorkStep.Publish(response.Order, token);

        return response;
    }
}

public sealed class CreateOrderResponse {
    public Order Order { get; set; }
    public IEnumerable<Error> Errors { get; set; }
    public bool IsOrderPlaced { get; internal set; }
}

public sealed record Error(string PropertyName, string ErrorMessage);
public sealed record CreateOrderRequest(Guid CustomerId, IEnumerable<CreateOrderLineRequest> Lines);

public sealed record CreateOrderLineRequest(Guid ProductId, int Quantity, decimal UnitPrice);
