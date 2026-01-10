using Business.Experts.Shared.Business.Events;
using BusinessExperts.OrderBusinessExpert.Shared.Business.Domain;
using Common.Events;

namespace BusinessExperts.OrderBusinessExpert.WorkFlows.PlaceOrderBusinessWorkFlow.WorkSteps;

public class PublishWorkStep(IBusinessEventPublisher bus) {
    public  Task<bool> Publish(Order order, CancellationToken token) {

        var businessEvent = new OrderPlaced(
            order.Id,
            order.CustomerId,
            order.Lines.Sum(l => l.UnitPrice * l.Quantity));
         
        return  bus.Publish(businessEvent, token);
    }
}
