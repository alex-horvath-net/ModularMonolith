namespace Accounts.Register.UserStory;

internal sealed class Product<TResult> {
    internal TResult Response { get; init; } = default!;
    internal IReadOnlyList<RegistrationWorkStep> ExecutedBusinessWorkSteps { get; init; } = [];

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
