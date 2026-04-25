namespace Accounts.Login.WorkSteps;

internal sealed class Normalize {
    public bool Run(Context context) {
        context.NormalizedEmail = context.Email!.Trim().ToLowerInvariant();

        return true;
    }
}
