using Features.Accounts.Infrastructure;

namespace Features.Accounts.Slices.Register.WorkSteps;

internal sealed class Save(IAccountRepository repsoitory) {
    public async Task<bool> Run(UserStory.Context context) {
        await repsoitory.CreateAccount(context.Account!, context.Token);
        return true;
    }
}
