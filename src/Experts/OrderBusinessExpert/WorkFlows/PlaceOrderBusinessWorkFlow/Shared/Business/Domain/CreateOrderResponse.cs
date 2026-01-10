using Business.Experts.OrderBusinessExpert.Shared.Business.Domain;
using Business.Experts.Shared.Business.Domain;

namespace Business.Experts.OrderBusinessExpert.WorkFlows.PlaceOrderBusinessWorkFlow.Shared.Business.Domain;

public sealed class CreateOrderResponse {
    public Order Order { get; set; }
    public IEnumerable<Error> Errors { get; set; }
    public bool IsOrderPlaced { get; set; }
}
