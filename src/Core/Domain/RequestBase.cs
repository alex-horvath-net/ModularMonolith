namespace Core.Domain;

public record RequestBase(
    Guid CorrelationId);
