using Experts.OrderExpert.Shared.Business.Domain;
using Experts.Shared.Business.Domain;

namespace Experts.OrderExpert.PlaceOrderFlow.Shared.Business.Domain;

public sealed class CreateOrderResponse {
    public Order? Order { get; set; }
    public IEnumerable<Error> Errors { get; set; } = [];
}
