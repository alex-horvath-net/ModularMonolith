using Core.Domain;
using Core.Infrastructure;

namespace Accounts.Register;

internal sealed class Normalize(IClock clock) : WorkStep<Context>(clock) {
    protected override Task Run(Context context) {
        context.NormalizedRequest = context.Request with {
            Email = NormalizeEmai(context),
            UserName = NormalizeUserName(context),
            Roles = NormalizeRoles(context),
        };

        return Task.CompletedTask;
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
