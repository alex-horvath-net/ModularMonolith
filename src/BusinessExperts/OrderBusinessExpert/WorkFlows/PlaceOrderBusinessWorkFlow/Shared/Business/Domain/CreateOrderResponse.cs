using BusinessExperts.OrderBusinessExpert.Shared.Business.Domain;
using BusinessExperts.Shared.Business.Domain;

namespace BusinessExperts.OrderBusinessExpert.WorkFlows.PlaceOrderBusinessWorkFlow.Shared.Business.Domain;

public sealed class CreateOrderResponse {
    public Order Order { get; set; }
    public IEnumerable<Error> Errors { get; set; }
    public bool IsOrderPlaced { get; set; }
}
