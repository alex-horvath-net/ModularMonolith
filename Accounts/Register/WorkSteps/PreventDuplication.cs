using Accounts.Core.Infrastructure;
using Accounts.Register.UserStory;
using Core.Domain;
using Core.Infrastructure;

namespace Accounts.Register.WorkSteps;

internal sealed class PreventDuplication(
    IAccountRepository repository,
    IClock clock,
    ILogger<PreventDuplication> logger) : WorkStep<Context>(clock, logger) {
    public async Task Run(Context context) {
        context.ExecutedBusinessWorkSteps.Add(RegistrationWorkStep.PreventDuplication);

        context.MachingAccount = await repository.FindAccountByEmail(context.NormalizedRequest!.Email, context.Token);
        if (context.MachingAccount is not null)
            throw new InvalidOperationException(Constants.AccountAlreadyExists);
    }
}
