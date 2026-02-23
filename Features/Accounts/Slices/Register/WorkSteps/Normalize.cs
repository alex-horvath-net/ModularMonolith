using Common.Tasks;

namespace Features.Accounts.Slices.Register.WorkSteps;

internal sealed class Normalize {
    public bool Run(UserStory.UserStoryContext context) {
        context.NormalizedRequest = context.Request with {
            Email = context.Request.Email.Trim().ToLowerInvariant(),
            UserName = context.Request.UserName.Trim().ToLowerInvariant(),
            Roles = context.Request.Roles
                .Where(role => !string.IsNullOrWhiteSpace(role))
                .Select(role => role.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray()
        };

        return true;
    }
}
