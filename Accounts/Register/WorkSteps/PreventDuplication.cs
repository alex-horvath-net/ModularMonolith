using Accounts.Core.Infrastructure;
using Accounts.Register.UserStory;

namespace Accounts.Register.WorkSteps;

internal sealed class PreventDuplication(IAccountRepository repository) {
    public async Task Run(Context context) {
        context.ExecutedBusinessWorkSteps.Add(RegistrationWorkStep.PreventDuplication);

        context.MachingAccount = await repository.FindAccountByEmail(context.NormalizedRequest!.Email, context.Token);
        if (context.MachingAccount is not null)
            throw new InvalidOperationException(Constants.AccountAlreadyExists);
    }
}
