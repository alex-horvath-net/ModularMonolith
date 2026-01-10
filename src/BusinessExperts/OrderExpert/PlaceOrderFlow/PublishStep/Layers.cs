using Experts.OrderExpert.Shared.Business.Domain;
using Experts.Shared.Business.Events;

namespace Experts.OrderExpert.PlaceOrderFlow.PublishStep;

public class Business(Business.IAdapter adapter) {
    public Task Run(Order order, CancellationToken token) =>
        adapter.PublishOrderPlaced(order, token);

    public interface IAdapter {
        Task PublishOrderPlaced(Order order, CancellationToken token);
    }
}

public class Adapter(IBusinessEventPublisher bus) : Business.IAdapter {
    public Task PublishOrderPlaced(Order order, CancellationToken token) {

        var businessEvent = new OrderPlaced(
            order.Id,
            order.CustomerId,
            order.Total);

        return bus.Publish(businessEvent, token);
    }
}