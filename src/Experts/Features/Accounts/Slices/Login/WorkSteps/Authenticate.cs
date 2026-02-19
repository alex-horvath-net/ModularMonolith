using Business.Features.Accounts.Infrastructure;

namespace Business.Features.Accounts.Slices.Login.WorkSteps;

internal sealed class Authenticate(IAccountRepository repsository, IHasher hasher) {
    public async Task<bool> Run(UserStory.Context context) {

        context.Account = await repsository.FindAccountByEmail(context.NormalizedEmail!, context.Token);

        if (context.Account is null) {
            context.Response.ErrorMessage = UserStoryConstants.AccountNotFound;
            return false;
        }

        if (context.Account.IsLocked) {
            context.Response.ErrorMessage = UserStoryConstants.AccontLocked;
            return false;
        }

        if (!hasher.Verify(context.Password!, context.Account.PasswordHash)) {
            context.Response.ErrorMessage = UserStoryConstants.InvalidPassword;
            return false;
        }

        context.Response.AuthenticationId = context.Account.Id;
        context.Response.UserName = context.Account.UserName;

        return true;
    }
}
