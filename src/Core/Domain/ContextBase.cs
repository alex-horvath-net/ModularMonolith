namespace Core.Domain;

public abstract record ContextBase() {
    public Guid? CorellationId { get; protected internal set; }
    public Guid? RequestId { get; protected internal set; }
    public List<string> WorkSteps { get; internal set; } = [];
    public Exception? Exception { get; internal set; }
}