using Experts.OrderBusinessExpert.BusinessWorkFlows.Shared.Business.Domain;
using Experts.Shared.Business.Domain;

namespace Experts.OrderBusinessExpert.BusinessWorkFlows.PlaceOrderBusinessWorkFlow.BusinessWorkSteps.Shared.Business.Domain;

public sealed class CreateOrderResponse {
    public Order Order { get; set; }
    public IEnumerable<Error> Errors { get; set; }
    public bool IsOrderPlaced { get; set; }
}
