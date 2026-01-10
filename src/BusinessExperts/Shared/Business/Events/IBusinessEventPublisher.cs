namespace Experts.Shared.Business.Events;

public interface IBusinessEventPublisher {
    Task<bool> Publish<T>(T businessEvent, CancellationToken token = default) where T : IBusinessEvent;
}
