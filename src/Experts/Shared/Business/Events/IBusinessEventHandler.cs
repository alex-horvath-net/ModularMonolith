namespace Experts.Shared.Business.Events;

public interface IBusinessEventHandler<in TEvent> where TEvent : IBusinessEvent {
    Task Handle(TEvent businessEvent, CancellationToken token = default);
}
