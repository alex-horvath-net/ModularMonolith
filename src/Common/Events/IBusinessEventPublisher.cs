namespace Common.Events;

public interface IBusinessEventPublisher {
    Task Publish<T>(T businessEvent, CancellationToken token = default) where T : IBusinessEvent;
}
