using Features.Accounts.Infrastructure;

namespace Features.Accounts.Slices.Register.WorkSteps;

internal sealed class PreventDuplication(IAccountRepository repository) {
    public async Task<bool> Run(UserStory.UserStoryContext context) {

        context.MachingAccount = await repository.FindAccountByEmail(context.NormalizedRequest!.Email, context.Token);
        if (context.MachingAccount is not null)
            throw new InvalidOperationException(UserStoryConstants.AccountAlreadyExists);

        return true;
    }
}
