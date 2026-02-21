namespace Common.Tasks;

public static class MonadExtensions {

    public static Task<T> Unit<T>(this T value) =>
        value.ToTask();

    public static Task<TOutput> Bind<TInput, TOutput>(this Task<TInput> noneBlockingInputQuery, Func<TInput, Task<TOutput>> blockingQueryOfNoneBlockingOutputQuery) =>
        noneBlockingInputQuery.Map(blockingQueryOfNoneBlockingOutputQuery);

    public static Task<TOutput> SelectManyByUnitandBind<TInput, TMidle, TOutput>(
        this Task<TInput> slowInputFactory,
        Func<TInput, Task<TMidle>> fastFactoryOfSlowMidleFactory,
        Func<TInput, TMidle, TOutput> fastOutputFactory) =>
            slowInputFactory.Bind(input => {
                return fastFactoryOfSlowMidleFactory(input).Bind(midle => fastOutputFactory(input, midle).Unit());
            });

    public static async Task<TOutput> SelectMany<TInput, TMidle, TOutput>(
        this Task<TInput> slowInputFactory,
        Func<TInput, Task<TMidle>> fastFactoryOfSlowMidleFactory,
        Func<TInput, TMidle, TOutput> fastOutputFactory) {
        var input = await slowInputFactory;
        var SlowMidleFactory = fastFactoryOfSlowMidleFactory(input);
        var middle = await SlowMidleFactory;
        var output = fastOutputFactory(input, middle);
        return output;
    }

    public static async Task<TOutput> SelectMany2<TInput, TMidle, TOutput>(
       this Task<TInput> noneBlockingInputQuery,
       Func<TInput, Task<TMidle>> fastFactoryOfSlowMidleFactory,
       Func<TInput, TMidle, TOutput> fastOutputFactory) {
        var input = await noneBlockingInputQuery;
        var SlowMidleFactory = fastFactoryOfSlowMidleFactory(input);
        var middle = await SlowMidleFactory;
        var output = fastOutputFactory(input, middle);
        return output;
    }

    public static async Task<TOutput> Select<TInput, TOutput>(
        this Task<TInput> slowInputFactory,
        Func<TInput, TOutput> fastOutputFactory) {
        var input = await slowInputFactory;
        var output = fastOutputFactory(input);
        return output;
    }


}
