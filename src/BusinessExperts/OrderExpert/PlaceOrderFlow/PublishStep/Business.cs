using Common.Events;
using Experts.Shared.Business.Events;

namespace Experts.OrderExpert.PlaceOrderFlow.PublishStep;

public class Business(IBusinessEventPublisher bus) {
    public Task<bool> Run(OrderExpert.Shared.Business.Domain.Order order, CancellationToken token) {

        var businessEvent = new OrderPlaced(
            order.Id,
            order.CustomerId,
            order.Lines.Sum(l => l.UnitPrice * l.Quantity));

        return bus.Publish(businessEvent, token);
    }
}
