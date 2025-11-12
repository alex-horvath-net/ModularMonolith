using Microsoft.Extensions.DependencyInjection;

namespace Common;

// events, error type, nothing business-specific
// You don’t need a broker to keep modules decoupled.
// Start with a tiny in-process dispatcher.
public interface IBusinessEvent { }

public interface IBusinessEventHandler<in TEvent> where TEvent : IBusinessEvent {
    Task Handle(TEvent businessEvent, CancellationToken token = default);
}

public interface IBusinessEventPublisher {
    Task Publish<T>(T businessEvent, CancellationToken token = default) where T : IBusinessEvent;
}

public sealed class InProcessEventBus(IServiceProvider services) : IBusinessEventPublisher {
    public async Task Publish<T>(T businessEvent, CancellationToken token = default) where T : IBusinessEvent {
        var handlers = services.GetServices<IBusinessEventHandler<T>>();
        foreach (var handler in handlers)
            await handler.Handle(businessEvent, token);
    }
}

public static class BusinessEventExtensions {
    public static IServiceCollection AddCommon(this IServiceCollection services) =>
        services.AddScoped<IBusinessEventPublisher, InProcessEventBus>();
}
