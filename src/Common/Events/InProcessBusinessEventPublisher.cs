using Microsoft.Extensions.DependencyInjection;

namespace Common.Events;

public sealed class InProcessBusinessEventPublisher(IServiceProvider services) : IBusinessEventPublisher {
    public async Task<bool> Publish<T>(T businessEvent, CancellationToken token = default) where T : IBusinessEvent {
        var handlers = services.GetServices<IBusinessEventHandler<T>>();
        foreach (var handler in handlers)
            await handler.Handle(businessEvent, token);
        
        return true;
    }
}
