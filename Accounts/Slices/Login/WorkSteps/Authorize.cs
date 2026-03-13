using Core.Domain.Tasks;

namespace Accounts.Slices.Login.WorkSteps;

internal sealed class Authorize() {
    public static readonly string AccountNotFound = "Account not found";
    public Task<bool> Run(UserStory.Context context) {

        context.Response.Roles = context.Account!.Roles.ToList();

        return true.ToTask();
    }
}