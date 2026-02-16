using Common.Tasks;

namespace Business.Experts.SecurityOfficer.UserStories.Register.WorkSteps;
internal class Normalize {
    public bool Run(UserStory.UserStoryContext context) {
        context.NormalizedRequest =
            context.Request with {
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
