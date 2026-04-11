using Accounts.Core.Infrastructure;
using Accounts.Register.UserStory;
using Core.Domain;
using Core.Infrastructure;

namespace Accounts.Register.WorkSteps;

internal sealed class Save(
    IAccountRepository repsoitory,
    IClock clock,
    IGuid guid,
    ILogger<Save> logger) : WorkStep<Context>(clock, guid, logger) {
    protected override async Task Run(Context context) {
        context.ExecutedBusinessWorkSteps.Add(RegistrationWorkStep.SaveIdentity);
        await repsoitory.CreateAccount(context.Account!, context.Token);
    }
}
