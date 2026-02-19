using Business.Experts.SecurityOfficer.Domain;
using Business.Features.Accounts.Infrastructure;

namespace Business.Features.Accounts.Slices.Register.WorkSteps;
internal class Create(IHasher hasher, IClock clock) {
    public bool Run(UserStory.UserStoryContext context) {
        var hash = hasher.Generate(context.NormalizedRequest!.Password);
        var now = clock.UtcNow;

        context.Account = new Account(
            Id: Guid.NewGuid(),
            Email: context.NormalizedRequest!.Email,
            UserName: context.NormalizedRequest.UserName,
            PasswordHash: hash,
            Roles: context.Request.Roles.ToHashSet(),
            IsLocked: false,
            CreatedAtUtc: now);

        return true;
    }

}


