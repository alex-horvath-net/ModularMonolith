using Business.Experts.SecurityOfficer.Domain;

namespace Business.Experts.SecurityOfficer.UserStories.Register.WorkSteps;
internal class Create(Create.IHasher hasher, Create.IClock clock) {
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

    public interface IClock {
        DateTime UtcNow { get; }
    }

    public interface IHasher {
        string Generate(string password);
    }

}


