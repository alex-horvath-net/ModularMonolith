using Accounts.Core.Infrastructure;
using Core.Domain;
using Core.Infrastructure;

namespace Accounts.Register.UserStory;

internal sealed class Save(
    IAccountRepository repsoitory,
    IClock clock,
    IGuidGenerator guid,
    ILogger<UserStory> logger) : WorkStep<Context>(clock, guid, logger) {
    protected override async Task Run(Context context) {
        context.ExecutedBusinessWorkSteps.Add(RegistrationWorkStep.SaveIdentity);
        await repsoitory.CreateAccount(context.Account!, context.Token);
    }
}
