namespace Features.Accounts.Slices.Login.WorkSteps;

internal sealed class Normalize {
    public bool Run(UserStory.Context context) {
        context.NormalizedEmail = context.Email!.Trim().ToLowerInvariant();

        return true;
    }
}
