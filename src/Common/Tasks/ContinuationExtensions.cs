namespace Common.Tasks;

public static class ContinuationExtensions {
    public static async Task Then(this Task slowOperation, Action fastOperation) {
        await slowOperation;
        fastOperation();
    }
    public static async Task Then(this Task slowOperation, Func<Task> fastFactoryOfNextSlowOperation) {
        await slowOperation;
        var nextSlowOperation = fastFactoryOfNextSlowOperation();
        await nextSlowOperation;
    }

    // e.g. read a record from db in data format then map it to domain format.
    public static async Task<TOutput> Then<TOutput>(this Task slowOperation, Func<TOutput> fastOutputFactory) {
        await slowOperation;
        var output = fastOutputFactory();
        return output;
    }
    public static async Task<TOutput> Then<TOutput>(this Task slowOperation, Func<Task<TOutput>> fastFactoryOfSlowOutputFactory) {
        await slowOperation;
        var slowOutputFactory = fastFactoryOfSlowOutputFactory();
        var output = await slowOutputFactory;
        return output;
    }

    // e.g. read a record from db in data format then refine its content.
    //
    public static async Task Then<TInput>(this Task<TInput> slowInputFactory, Action<TInput> fastInputOperation) {
        var input = await slowInputFactory;
        fastInputOperation(input);
    }
    public static async Task Then<TInput>(this Task<TInput> slowInputFactory, Func<TInput, Task> fastFactoryOfSlowOperation) {
        var input = await slowInputFactory;
        var slowOperation = fastFactoryOfSlowOperation(input);
        await slowOperation;
    }

    // e.g. read a record from db in data format then map it to domain format.
    public static async Task<TOutput> Then<TInput, TOutput>(this Task<TInput> slowInputFactory, Func<TInput, TOutput> fastOutputFactory) {
        var input = await slowInputFactory;
        var output = fastOutputFactory(input);
        return output;
    }
    public static async Task<TOutput> Then<TInput, TOutput>(this Task<TInput> slowInputFactory, Func<TInput, Task<TOutput>> fastfactoryOfSlowOutputFactory) {
        var input = await slowInputFactory;
        var slowOutputFactory = fastfactoryOfSlowOutputFactory(input);
        var output = await slowOutputFactory;
        return output;
    }
}
