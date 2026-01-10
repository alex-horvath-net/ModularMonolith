using Business.Experts.OrderBusinessExpert.Shared.Business.Domain;

namespace Business.Experts.OrderBusinessExpert.WorkFlows.PlaceOrderBusinessWorkFlow.Shared.Business.Domain;

public sealed record CreateOrderRequest(Guid CustomerId, IEnumerable<CreateOrderLineRequest> Lines);
