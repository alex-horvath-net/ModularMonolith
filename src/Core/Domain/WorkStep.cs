using Core.Infrastructure;

namespace Core.Domain;

public abstract class WorkStep<TContext>(IClock clock, ILogger<WorkStep<TContext>> logger) where TContext : ContextBase {
    private const string MessageTemplate = "WorkStep {WorkStep} is {Status} at {Time}. CorellationId is {CorellationId}.";

    public async Task Execute(TContext context) {
        var workStepName = GetType().Name;
        try {
            logger.LogInformation(MessageTemplate, workStepName, "Started", clock.UtcNow, context.CorellationId);

            await Run(context);

            logger.LogInformation(MessageTemplate, workStepName, "Completed", clock.UtcNow, context.CorellationId);

        } catch (OperationCanceledException oce) {
            logger.LogWarning(oce, MessageTemplate, workStepName, "Canceled", clock.UtcNow, context.CorellationId);
        } catch (Exception ex) {
            logger.LogError(ex, MessageTemplate, workStepName, "Failed", clock.UtcNow, context.CorellationId);
            throw;
        }
    }

    protected virtual Task Run(TContext context) => Task.CompletedTask;
}