namespace Common.Tasks;

public static class CovertExtensions {

    public static Task<TOutput> ToTask<TOutput>(this TOutput output) =>
        Task.FromResult(output);

    public static Task<TOutput> ToTask<TOutput>(this Func<TOutput> fastOutputFactory) {
        var output = fastOutputFactory();
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
