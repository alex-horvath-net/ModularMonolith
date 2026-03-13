using Accounts.Infrastructure;
using Accounts.Slices.Register.UserStory;

namespace Accounts.Slices.Register.WorkSteps;

internal sealed class PreventDuplication(IAccountRepository repository) {
    public async Task<bool> Run(Context context) {

        context.MachingAccount = await repository.FindAccountByEmail(context.NormalizedRequest!.Email, context.Token);
        if (context.MachingAccount is not null)
            throw new InvalidOperationException(Constants.AccountAlreadyExists);

        return true;
    }
}
