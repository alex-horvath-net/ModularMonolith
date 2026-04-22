namespace Core.Domain;

public abstract record ContextBase(Guid CorellationId, Guid RequestId) {
    public List<string> WorkSteps { get; internal set; } = [];
    public Exception? Exception { get; internal set; }
}