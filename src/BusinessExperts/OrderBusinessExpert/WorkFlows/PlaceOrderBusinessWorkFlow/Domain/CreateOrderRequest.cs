using Experts.OrderBusinessExpert.Shared.Business.Domain;

namespace Experts.OrderBusinessExpert.WorkFlows.PlaceOrderBusinessWorkFlow.Domain;

public sealed record CreateOrderRequest(Guid CustomerId, IEnumerable<OrderLine> Lines);
