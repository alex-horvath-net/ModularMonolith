namespace Business.Features.Accounts.Slices.Register.WorkSteps;

internal class BuildResponse() {
    public bool Run(UserStory.UserStoryContext context) {

        context.Response.AccountId = context.Account!.Id;
        context.Response.Email = context.Account.Email;
        context.Response.UserName = context.Account.UserName;
        context.Response.Roles = context.Account.Roles;

        return true;
    }
}