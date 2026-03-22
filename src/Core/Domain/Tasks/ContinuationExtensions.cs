namespace Core.Domain.Tasks;

public static class ContinuationExtensions {
    public static TOutput Map<TInput, TOutput>(this TInput input, Func<TInput, TOutput> blockingOutputQuery) {
        var output = blockingOutputQuery(input);
        return output;
    }
    public static async Task<TOutput> Map<TOutput>(this Task noneBlockingCommand, Func<TOutput> blockingOutputQuery) {
        await noneBlockingCommand;
        var output = blockingOutputQuery();
        return output;
    }
    public static async Task<TOutput> Map<TOutput>(this Task noneBlockingCommand, Func<Task<TOutput>> blockingQueryOfNoneBlockonOutputQuery) {
        await noneBlockingCommand;
        var noneBlockonOutputQuery = blockingQueryOfNoneBlockonOutputQuery();
        var output = await noneBlockonOutputQuery;
        return output;
    }
    public static async Task<TOutput> Map<TInput, TOutput>(this Task<TInput> noneBockingInputQuery, Func<TInput, TOutput> blockingOutputQuery) {
        var input = await noneBockingInputQuery;
        var output = blockingOutputQuery(input);
        return output;
    }
    public static async Task<TOutput> Map<TInput, TOutput>(this Task<TInput> noneBockingInputQuery, Func<TInput, Task<TOutput>> blockingQueryOfNoneBlockonOutputQuery) {
        var input = await noneBockingInputQuery;
        var noneBlockingOutputFactory = blockingQueryOfNoneBlockonOutputQuery(input);
        var output = await noneBlockingOutputFactory;
        return output;
    }

    public static TInput Then<TInput>(this TInput input, Action command) {
        command();
        return input;
    }
    public static TInput Then<TInput>(this TInput input, Action<TInput> command) {
        command(input);
        return input;
    }
    public static async Task<TInput> Then<TInput>(this TInput input, Func<Task> queryOfNoneBlockingCommand) {
        var noneBlockingCommand = queryOfNoneBlockingCommand();
        await noneBlockingCommand;
        return input;
    }
    public static async Task Then(this Task noneBlockingCommand, Action command) {
        await noneBlockingCommand;
        command();
    }
    public static async Task Then(this Task noneBlockingCommand, Func<Task> bockingQueryOfNoneBlockingCommand) {
        await noneBlockingCommand;
        var nextNoneBlockingCommand = bockingQueryOfNoneBlockingCommand();
        await nextNoneBlockingCommand;
    }

    public static async Task Then<TInput>(this Task<TInput> noneBockingInputQuery, Action<TInput> blockingInputCommand) {
        var input = await noneBockingInputQuery;
        blockingInputCommand(input);
    }
    public static async Task Then<TInput>(this Task<TInput> noneBockingInputQuery, Func<TInput, Task> bockingQueryOfNoneBlockingCommand) {
        var input = await noneBockingInputQuery;
        var noneBlockingCommand = bockingQueryOfNoneBlockingCommand(input);
        await noneBlockingCommand;
    }
}
