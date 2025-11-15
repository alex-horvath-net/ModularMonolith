using Microsoft.Extensions.DependencyInjection;

namespace Common;

public interface IBusinessEvent { }

public interface IBusinessEventHandler<in TEvent> where TEvent : IBusinessEvent {
    Task Handle(TEvent businessEvent, CancellationToken token = default);
}

public interface IBusinessEventPublisher {
    Task Publish<T>(T businessEvent, CancellationToken token = default) where T : IBusinessEvent;
}

public sealed class InProcessBusinessEventPublisher(IServiceProvider services) : IBusinessEventPublisher {
    public async Task Publish<T>(T businessEvent, CancellationToken token = default) where T : IBusinessEvent {
        var handlers = services.GetServices<IBusinessEventHandler<T>>();
        foreach (var handler in handlers)
            await handler.Handle(businessEvent, token);
    }
}
