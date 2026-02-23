namespace Core.Features.Accounts.Slices.Login.WorkSteps;

internal class Normalize {
    public bool Run(UserStory.Context context) {
        context.NormalizedEmail = context.Email!.Trim().ToLowerInvariant();

        return true;
    }
}
