using Business.Features.Accounts.Infrastructure;

namespace Business.Features.Accounts.Slices.Register.WorkSteps;

internal class PreventDuplication(IAccountRepository repository) {
    public async Task<bool> Run(UserStory.UserStoryContext context) {

        context.MachingAccount = await repository.FindAccountByEmail(context.NormalizedRequest!.Email, context.Token);
        if (context.MachingAccount is not null)
            throw new InvalidOperationException(UserStoryConstants.AccountAlreadyExists);

        return true;
    }
}
