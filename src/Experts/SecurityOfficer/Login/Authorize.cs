namespace Experts.SecurityOfficer.Login;

internal sealed class Authorize() {
    public static readonly string AccountNotFound = "Account not found";
    public async Task<bool> Run(UserStory.Context context) {
        if (context.Account is null) {
            context.Response.ErrorMessage = AccountNotFound;
            return false;
        }

        context.Response.Roles = context.Account.Roles.ToList();

        await Task.CompletedTask;

        return true;
    }
}