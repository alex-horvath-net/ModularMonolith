using Business.Contracts.Events;
using Business.OrderBusinessExpert.PlaceOrderBusinessWorkFlow.Domain;
using Common.Events;

namespace Business.OrderBusinessExpert.PlaceOrderBusinessWorkFlow.WorkSteps;

public class PublishWorkStep(IBusinessEventPublisher bus) {
    public  Task<bool> Publish(Order order, CancellationToken token) {

        var businessEvent = new OrderPlaced(
            order.Id,
            order.CustomerId,
            order.Lines.Sum(l => l.UnitPrice * l.Quantity));
         
        return  bus.Publish(businessEvent, token);
    }
}
