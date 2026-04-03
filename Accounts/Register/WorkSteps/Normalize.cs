using Accounts.Register.UserStory;

namespace Accounts.Register.WorkSteps;

internal sealed class Normalize {
    public void Run(Context context) {
        context.ExecutedBusinessWorkSteps.Add(RegistrationWorkStep.Normalization);
        context.NormalizedRequest = context.Request with {
            Email = NormalizeEmai(context),
            UserName = NormalizeUserName(context),
            Roles = NormalizeRoles(context),
        };
    }

    private string NormalizeEmai(Context context) => context.Request.Email
        .Trim()
        .ToLowerInvariant();

    private string NormalizeUserName(Context context) => context.Request.UserName
        .Trim();

    private string[] NormalizeRoles(Context context) => context.Request.Roles
        .Where(role => !string.IsNullOrWhiteSpace(role))
        .Select(role => role.Trim())
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .ToArray();
}
