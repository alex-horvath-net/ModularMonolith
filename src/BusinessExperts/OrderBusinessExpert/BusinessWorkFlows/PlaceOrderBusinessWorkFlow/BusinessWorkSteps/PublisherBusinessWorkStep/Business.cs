using Experts.Shared.Business.Events;
using Common.Events;
using Experts.OrderBusinessExpert.BusinessWorkFlows.Shared.Business.Domain;

namespace Experts.OrderBusinessExpert.BusinessWorkFlows.PlaceOrderBusinessWorkFlow.BusinessWorkSteps.PublisherBusinessWorkStep;

public class Business(IBusinessEventPublisher bus) {
    public Task<bool> Publish(Order order, CancellationToken token) {

        var businessEvent = new OrderPlaced(
            order.Id,
            order.CustomerId,
            order.Lines.Sum(l => l.UnitPrice * l.Quantity));

        return bus.Publish(businessEvent, token);
    }
}
