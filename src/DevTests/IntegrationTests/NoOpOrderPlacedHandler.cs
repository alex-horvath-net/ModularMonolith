using BusinessExperts.Contracts.Events;
using Common.Events;

namespace DevTests.IntegrationTests;

public sealed class NoOpOrderPlacedHandler2 : IBusinessEventHandler<OrderPlaced> {
    public Task Handle(OrderPlaced businessEvent, CancellationToken token = default) => Task.CompletedTask;
}
