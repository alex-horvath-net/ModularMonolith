using Experts.Shared.Business.Domain;

namespace Experts.Order.PlaceOrderBusinessWorkFlow.Shared.Business.Domain;

public sealed class CreateOrderResponse {
    public Order.Shared.Business.Domain.Order Order { get; set; }
    public IEnumerable<Error> Errors { get; set; }
    public bool IsOrderPlaced { get; set; }
}
