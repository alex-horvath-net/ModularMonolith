namespace Accounts.Register.UserStory;

public sealed class Product<TResult> {
    public TResult Response { get; init; } = default!;
    public IReadOnlyList<RegistrationWorkStep> ExecutedBusinessWorkSteps { get; init; } = [];

    private static readonly string executedBusinessWorkStepsKey = $"{typeof(Product<TResult>).FullName}.{nameof(ExecutedBusinessWorkSteps)}";

    internal static Product<TResult> FromContext(Context context, TResult response) {
        ArgumentNullException.ThrowIfNull(context);

        return new() {
            Response = response,
            ExecutedBusinessWorkSteps = [.. context.ExecutedBusinessWorkSteps],
        };
    }

    internal static Product<TResult> FromException(Exception exception) {
        ArgumentNullException.ThrowIfNull(exception);

        return new() {
            ExecutedBusinessWorkSteps = exception.Data[executedBusinessWorkStepsKey] as IReadOnlyList<RegistrationWorkStep> ?? [],
        };
    }
}
