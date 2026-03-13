using Core.Infrastructure;

namespace Features.Accounts.Slices.Register.WorkSteps;

internal sealed class Create(IHasher hasher, IClock clock) {
    public bool Run(UserStory.Context context) {
        context.Account = new(
            Id: Guid.NewGuid(),
            Email: context.NormalizedRequest!.Email,
            UserName: context.NormalizedRequest.UserName,
            PasswordHash: hasher.Generate(context.NormalizedRequest!.Password),
            Roles: context.NormalizedRequest.Roles.ToHashSet(StringComparer.OrdinalIgnoreCase),
            IsLocked: false,
            CreatedAtUtc: clock.UtcNow);

        return true;
    }

}
