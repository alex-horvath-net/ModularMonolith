using Microsoft.Extensions.DependencyInjection;

namespace Core.Events;

public sealed class InProcessBusinessEventPublisher(IServiceProvider services) : IBusinessEventPublisher {
    public async Task Publish<T>(T businessEvent, CancellationToken token = default) where T : IBusinessEvent {
        var handlers = services.GetServices<IBusinessHandler<T>>();
        foreach (var handler in handlers)
            await handler.Handle(businessEvent, token);
    }
}
