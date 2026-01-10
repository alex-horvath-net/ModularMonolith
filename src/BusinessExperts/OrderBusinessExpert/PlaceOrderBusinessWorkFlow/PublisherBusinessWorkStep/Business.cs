using Common.Events;
using Experts.Business.Shared.Events;
using Experts.OrderBusinessExpert.Shared.Business.Domain;

namespace Experts.OrderBusinessExpert.PlaceOrderBusinessWorkFlow.PublisherBusinessWorkStep;

public class Business(IBusinessEventPublisher bus) {
    public Task<bool> Publish(Order order, CancellationToken token) {

        var businessEvent = new OrderPlaced(
            order.Id,
            order.CustomerId,
            order.Lines.Sum(l => l.UnitPrice * l.Quantity));

        return bus.Publish(businessEvent, token);
    }
}
