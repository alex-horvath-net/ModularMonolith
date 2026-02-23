using Core.Features.Accounts.Domain;
using Core.Infrastructure;

namespace Core.Features.Accounts.Slices.Register.WorkSteps;

internal class Create(IHasher hasher, IClock clock) {
    public bool Run(UserStory.UserStoryContext context) {
        var hash = hasher.Generate(context.NormalizedRequest!.Password);
        var now = clock.UtcNow;

        context.Account = new Account(
            Id: Guid.NewGuid(),
            Email: context.NormalizedRequest!.Email,
            UserName: context.NormalizedRequest.UserName,
            PasswordHash: hash,
            Roles: context.NormalizedRequest.Roles.ToHashSet(StringComparer.OrdinalIgnoreCase),
            IsLocked: false,
            CreatedAtUtc: now);

        return true;
    }

}
