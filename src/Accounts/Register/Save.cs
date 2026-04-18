using Accounts.Core.Infrastructure;
using Core.Domain;
using Core.Infrastructure;

namespace Accounts.Register;

internal sealed class Save(
    IAccountRepository repsoitory,
    IClock clock,
    IGuidGenerator guid) : WorkStep<Context>(clock, guid) {
    protected override async Task Run(Context context) {
        context.ExecutedBusinessWorkSteps.Add(RegistrationWorkStep.SaveIdentity);
        await repsoitory.CreateAccount(context.Account!, context.Token);
    }
}
