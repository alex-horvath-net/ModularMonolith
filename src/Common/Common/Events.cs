using Microsoft.Extensions.DependencyInjection;

namespace Common;
public interface IEvent { }

public interface IEventHandler<in TEvent> where TEvent : IEvent {
    Task Handle(TEvent businessEvent, CancellationToken ct = default);
}

public interface IEventBus {
    Task Publish<T>(T @event, CancellationToken ct = default) where T : IEvent;
}

public sealed class InProcessEventBus(IServiceProvider sp) : IEventBus {
    public async Task Publish<T>(T businessEvent, CancellationToken ct = default) where T : IEvent {
        var handlers = sp.GetServices<IEventHandler<T>>();
        foreach (var h in handlers)
            await h.Handle(businessEvent, ct);
    }
}

