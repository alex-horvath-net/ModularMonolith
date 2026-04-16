using Core.Infrastructure;

namespace Core.Domain;

public abstract class WorkStep<TContext>(IClock clock, IGuidGenerator guidGenerator, ILogger<WorkStep<TContext>> logger) where TContext : ContextBase {
    private const string MessageTemplate = "WorkStep {WorkStep} is {Status} at {Time}. CorellationId is {CorellationId}. RequestId is {RequestId}.";

    public async Task Execute(TContext context) {
        try {
            context.CorellationId ??= guidGenerator.New();
            context.RequestId ??= guidGenerator.New();
            context.WorkSteps.Add(GetType().Name);

            logger.LogInformation(MessageTemplate, context.WorkSteps.Last(), "Started", clock.UtcNow, context.CorellationId, context.RequestId);

            await Run(context);

            logger.LogInformation(MessageTemplate, context.WorkSteps.Last(), "Completed", clock.UtcNow, context.CorellationId, context.RequestId);

        } catch (OperationCanceledException oce) {
            logger.LogWarning(oce, MessageTemplate, context.WorkSteps.Last(), "Canceled", clock.UtcNow, context.CorellationId, context.RequestId);
            context.Exception = oce;
        } catch (Exception ex) {
            logger.LogError(ex, MessageTemplate, context.WorkSteps.Last(), "Failed", clock.UtcNow, context.CorellationId, context.RequestId);
            context.Exception = ex;
            throw;
        }
    }

    protected virtual Task Run(TContext context) => Task.CompletedTask;
}