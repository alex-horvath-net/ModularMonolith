namespace Core.Domain.Tasks;

public class WorkStep<TContext>(TContext context) {
    public WorkStep<TOutput> AddStep<TOutput>(Func<TContext, WorkStep<TOutput>> step) =>
        step(context);
}
