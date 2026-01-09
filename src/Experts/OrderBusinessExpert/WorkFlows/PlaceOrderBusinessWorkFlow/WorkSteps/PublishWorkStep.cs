using Common.Events;
using Experts.OrderBusinessExpert.Shared.Business.Domain;
using Experts.Shared.Business.Events;

namespace Experts.OrderBusinessExpert.WorkFlows.PlaceOrderBusinessWorkFlow.WorkSteps;

public class PublishWorkStep(IBusinessEventPublisher bus) {
    public  Task<bool> Publish(Order order, CancellationToken token) {

        var businessEvent = new OrderPlaced(
            order.Id,
            order.CustomerId,
            order.Lines.Sum(l => l.UnitPrice * l.Quantity));
         
        return  bus.Publish(businessEvent, token);
    }
}
