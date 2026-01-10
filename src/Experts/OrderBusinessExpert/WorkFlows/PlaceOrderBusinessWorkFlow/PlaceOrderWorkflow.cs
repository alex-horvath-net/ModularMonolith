using Business.Experts.OrderBusinessExpert.WorkFlows.PlaceOrderBusinessWorkFlow.Shared.Business.Domain;
using Business.Experts.OrderBusinessExpert.WorkFlows.PlaceOrderBusinessWorkFlow.WorkSteps;

namespace Business.Experts.OrderBusinessExpert.WorkFlows.PlaceOrderBusinessWorkFlow;

public class PlaceOrderWorkflow( 
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
