namespace Common.Tasks;

public static class CancellationExtensions {
    public static async Task<TOutput> WithCancellation<TOutput>(this Task<TOutput> slowOutputFactory, CancellationToken token) {
        // Create a task that completes when the token is cancelled
        var cancellationTask = new TaskCompletionSource<bool>();
        using (token.Register(() => cancellationTask.SetResult(true))) {
            var completedTask = await Task.WhenAny(slowOutputFactory, cancellationTask.Task);
            if (completedTask == cancellationTask.Task) {
                throw new OperationCanceledException(token);
            }
            var output = await slowOutputFactory; // Await the original task to propagate exceptions
            return output;
        }
    }

    public static async Task WithCancellation(this Task slowOperation, CancellationToken token) {
        // Create a task that completes when the token is cancelled
        var cancellationTask = new TaskCompletionSource<bool>();
        using (token.Register(() => cancellationTask.SetResult(true))) {
            var completedTask = await Task.WhenAny(slowOperation, cancellationTask.Task);
            if (completedTask == cancellationTask.Task) {
                throw new OperationCanceledException(token);
            }
            await slowOperation; // Await the original task to propagate exceptions
        }
    }
}

public class WorkStep<TContext>(TContext context) {
    public WorkStep<TOutput> AddStep<TOutput>(Func<TContext, WorkStep<TOutput>> step) =>
        step(context);
}
