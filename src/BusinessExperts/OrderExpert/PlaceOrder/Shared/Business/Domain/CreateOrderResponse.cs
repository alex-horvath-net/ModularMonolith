using Experts.Shared.Business.Domain;

namespace Experts.OrderExpert.PlaceOrder.Shared.Business.Domain;

public sealed class CreateOrderResponse {
    public Experts.OrderExpert.Shared.Business.Domain.Order Order { get; set; }
    public IEnumerable<Error> Errors { get; set; }
    public bool IsOrderPlaced { get; set; }
}
