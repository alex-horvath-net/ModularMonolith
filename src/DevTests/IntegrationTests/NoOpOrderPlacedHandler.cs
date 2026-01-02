using BusinessExperts.Contracts.Events;
using Common.Events;

namespace DevTests.IntegrationTests;

public abstract partial class BaseIntegrationTest {
    private sealed class NoOpOrderPlacedHandler : IBusinessEventHandler<OrderPlaced> {
        public Task Handle(OrderPlaced businessEvent, CancellationToken token = default) => Task.CompletedTask;
    }
}
