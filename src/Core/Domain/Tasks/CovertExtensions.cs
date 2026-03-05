namespace Core.Domain.Tasks;

public static class CovertExtensions {
    public static TOutput ToSynch<TOutput>(this Task<TOutput> blockingOutputQuery) =>
        blockingOutputQuery.GetAwaiter().GetResult();
    public static Task<TOutput> ToTask<TOutput>(this TOutput output) =>
        Task.FromResult(output);

    public static TOutput? ToNullable<TOutput>(this TOutput output) =>
        output;

    public static Task<TOutput> ToTask<TOutput>(this Func<TOutput> blockingOutputQuery) {
        var output = blockingOutputQuery();
        return Task.FromResult(output);
    }

    public static Task<TOutput> ToTask<TInput, TOutput>(this Func<TInput, TOutput> fastOutputFactory, TInput input) {
        var output = fastOutputFactory(input);
        return Task.FromResult(output);
    }

    public static Task ToTask<TInput>(this Action<TInput> fastInputOperation, TInput input) {
        fastInputOperation(input);
        return Task.CompletedTask;
    }

}
