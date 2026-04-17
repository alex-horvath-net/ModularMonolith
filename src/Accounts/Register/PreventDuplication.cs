using Accounts.Core.Infrastructure;
using Core.Domain;
using Core.Infrastructure;

namespace Accounts.Register;

internal sealed class PreventDuplication(
    IAccountRepository repository,
    IClock clock,
    IGuidGenerator guid,
    ILogger<UserStory> logger) : WorkStep<Context>(clock, guid, logger) {
    protected override async Task Run(Context context) {
        context.ExecutedBusinessWorkSteps.Add(RegistrationWorkStep.PreventDuplication);

        context.MachingAccount = await repository.FindAccountByEmail(context.NormalizedRequest!.Email, context.Token);
        if (context.MachingAccount is not null)
            throw new InvalidOperationException(Constants.AccountAlreadyExists);
    }
}
