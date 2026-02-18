namespace Common.Tasks;

public static class ContinuationExtensions {
    public static TOutput Map<TInput, TOutput>(this TInput input, Func<TInput, TOutput> blockingQuery) {
        var output = blockingQuery(input);
        return output;
    }
    public static async Task<TOutput> Map<TOutput>(this Task noneBlockingCommand, Func<TOutput> blockingQuery) {
        await noneBlockingCommand;
        var output = blockingQuery();
        return output;
    }
    public static async Task<TOutput> Map<TOutput>(this Task noneBlockingCommand, Func<Task<TOutput>> noneBockingQuery) {
        await noneBlockingCommand;
        var slowOutputFactory = noneBockingQuery();
        var output = await slowOutputFactory;
        return output;
    }
    public static async Task<TOutput> Map<TInput, TOutput>(this Task<TInput> noneBockingQuery, Func<TInput, TOutput> bockingQuery) {
        var input = await noneBockingQuery;
        var output = bockingQuery(input);
        return output;
    }
    public static async Task<TOutput> Map<TInput, TOutput>(this Task<TInput> slowInputFactory, Func<TInput, Task<TOutput>> noneBockingQuery) {
        var input = await slowInputFactory;
        var slowOutputFactory = noneBockingQuery(input);
        var output = await slowOutputFactory;
        return output;
    }


    public static TInput Then<TInput>(this TInput input, Action<TInput> blockingCommand) {
        blockingCommand(input);
        return input;
    }
    public static async Task<TInput> Then<TInput>(this TInput input, Func<Task> fastFactoryOfNextSlowOperation) {
        await fastFactoryOfNextSlowOperation();
        return input;
    }
    public static async Task Then(this Task noneBlockingCommand, Action fastOperation) {
        await noneBlockingCommand;
        fastOperation();
    }
    public static async Task Then(this Task noneBlockingCommand, Func<Task> fastFactoryOfNextSlowOperation) {
        await noneBlockingCommand;
        var nextSlowOperation = fastFactoryOfNextSlowOperation();
        await nextSlowOperation;
    }
    public static async Task Then<TInput>(this Task<TInput> slowInputFactory, Action<TInput> fastInputOperation) {
        var input = await slowInputFactory;
        fastInputOperation(input);
    }
    public static async Task Then<TInput>(this Task<TInput> slowInputFactory, Func<TInput, Task> fastFactoryOfSlowOperation) {
        var input = await slowInputFactory;
        var slowOperation = fastFactoryOfSlowOperation(input);
        await slowOperation;
    }
}
