using Accounts.Register.UserStory;
using Core.Domain;
using Core.Infrastructure;

namespace Accounts.Register.WorkSteps;

internal sealed class Create(
    IHasher hasher,
    IClock clock,
    ILogger<Create> logger) : WorkStep<Context>(clock, logger) {
    public override Task Run(Context context) {
        context.ExecutedBusinessWorkSteps.Add(RegistrationWorkStep.CreateIdentity);

        context.Account = new(
            Id: Guid.NewGuid(),
            Email: context.NormalizedRequest!.Email,
            UserName: context.NormalizedRequest.UserName,
            PasswordHash: hasher.Generate(context.NormalizedRequest!.Password),
            Roles: context.NormalizedRequest.Roles.ToHashSet(StringComparer.OrdinalIgnoreCase),
            IsLocked: false,
            CreatedAtUtc: clock.UtcNow);

        return Task.CompletedTask;
    }

}
