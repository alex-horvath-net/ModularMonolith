using Accounts.Register.UserStory;

namespace Accounts.Register.WorkSteps;

internal sealed class Normalize {
    public bool Run(Context context) {
        context.NormalizedRequest = context.Request with {
            Email = context.Request.Email.Trim().ToLowerInvariant(),
            UserName = context.Request.UserName.Trim(),
            Roles = context.Request.Roles
                .Where(role => !string.IsNullOrWhiteSpace(role))
                .Select(role => role.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray()
        };

        return true;
    }
}
