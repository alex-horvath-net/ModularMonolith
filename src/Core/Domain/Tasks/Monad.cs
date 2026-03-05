namespace Core.Domain.Tasks;

public class Monad<TInput>(TInput input) {
    public Monad<TOutput> Map<TOutput>(Func<TInput, TOutput> outputFactory) {
        var output = outputFactory(input);
        return new Monad<TOutput>(output);
    }
}

public class Maybe<TInput>(TInput? input = null) where TInput : class {

    public Maybe<TOutput> Bind<TOutput>(Func<TInput, Maybe<TOutput>> factory) where TOutput : class =>
        input == null ? new Maybe<TOutput>() : factory(input);

}