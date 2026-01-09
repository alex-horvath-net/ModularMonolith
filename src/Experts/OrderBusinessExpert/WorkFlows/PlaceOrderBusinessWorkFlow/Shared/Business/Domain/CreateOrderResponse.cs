using Experts.OrderBusinessExpert.Shared.Business.Domain;
using Experts.Shared.Business.Domain;

namespace Experts.OrderBusinessExpert.WorkFlows.PlaceOrderBusinessWorkFlow.Shared.Business.Domain;

public sealed class CreateOrderResponse {
    public Order Order { get; set; }
    public IEnumerable<Error> Errors { get; set; }
    public bool IsOrderPlaced { get; set; }
}
