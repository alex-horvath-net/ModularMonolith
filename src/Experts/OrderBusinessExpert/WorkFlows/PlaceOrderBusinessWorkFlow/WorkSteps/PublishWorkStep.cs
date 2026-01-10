using Business.Experts.OrderBusinessExpert.Shared.Business.Domain;
using Business.Experts.Shared.Business.Events;
using Common.Events;

namespace Business.Experts.OrderBusinessExpert.WorkFlows.PlaceOrderBusinessWorkFlow.WorkSteps;

public class PublishWorkStep(IBusinessEventPublisher bus) {
    public  Task<bool> Publish(Order order, CancellationToken token) {

        var businessEvent = new OrderPlaced(
            order.Id,
            order.CustomerId,
            order.Lines.Sum(l => l.UnitPrice * l.Quantity));
         
        return  bus.Publish(businessEvent, token);
    }
}
