namespace Core.Domain.Tasks;

public static class CancellationExtensions {
    public static async Task<TOutput> WithCancellation<TOutput>(this Task<TOutput> noneBlockingOutputQuery, CancellationToken token) {
        var cancellationTask = new TaskCompletionSource<bool>();
        using (token.Register(() => cancellationTask.SetResult(true))) {
            var completedTask = await Task.WhenAny(noneBlockingOutputQuery, cancellationTask.Task);
            if (completedTask == cancellationTask.Task) {
                throw new OperationCanceledException(token);
            }
            var output = await noneBlockingOutputQuery; // Await the original task to propagate exceptions
            return output;
        }
    }

    public static async Task WithCancellation(this Task noneBlockingCommand, CancellationToken token) {
        // Create a task that completes when the token is cancelled
        var cancellationTask = new TaskCompletionSource<bool>();
        using (token.Register(() => cancellationTask.SetResult(true))) {
            var completedTask = await Task.WhenAny(noneBlockingCommand, cancellationTask.Task);
            if (completedTask == cancellationTask.Task) {
                throw new OperationCanceledException(token);
            }
            await noneBlockingCommand; // Await the original task to propagate exceptions
        }
    }
}
