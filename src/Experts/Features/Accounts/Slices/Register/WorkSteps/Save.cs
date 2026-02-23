using Core.Features.Accounts.Infrastructure;

namespace Core.Features.Accounts.Slices.Register.WorkSteps;

internal class Save(IAccountRepository repsoitory) {
    public async Task<bool> Run(UserStory.UserStoryContext context) {

        await repsoitory.CreateAccount(context.Account!, context.Token);

        return true;
    }
}
