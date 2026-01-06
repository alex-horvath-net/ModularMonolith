using Business.MemberApplicationUser.Contracts.Events;
using Business.MemberApplicationUser.OrderBusinessExpert.CreateOrderWorkFlow.Infrastructure.Data.Models;
using Common.Events;

namespace Business.MemberApplicationUser.OrderBusinessExpert.CreateOrderWorkFlow;

public class PublishWorkStep(IBusinessEventPublisher bus) {
    public  Task<bool> Publish(Order order, CancellationToken token) {

        var businessEvent = new OrderPlaced(
            order.Id,
            order.CustomerId,
            order.Lines.Sum(l => l.UnitPrice * l.Quantity));
         
        return  bus.Publish(businessEvent, token);
    }
}
