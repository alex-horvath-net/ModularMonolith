namespace Business.Events;

public interface IBusinessHandler<in TEvent> where TEvent : IBusinessEvent {
    Task Handle(TEvent businessEvent, CancellationToken token = default);
}
