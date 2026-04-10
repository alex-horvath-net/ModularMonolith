using Accounts.Core.Infrastructure;
using Accounts.Register.UserStory;
using Core.Domain;
using Core.Infrastructure;

namespace Accounts.Register.WorkSteps;

internal sealed class Save(
    IAccountRepository repsoitory,
    IClock clock,
    ILogger<Save> logger) : WorkStep<Context>(clock, logger) {
    public async Task<bool> Run(Context context) {
        context.ExecutedBusinessWorkSteps.Add(RegistrationWorkStep.SaveIdentity);
        await repsoitory.CreateAccount(context.Account!, context.Token);
        return true;
    }
}
