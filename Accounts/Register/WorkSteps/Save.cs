using Accounts.Core.Infrastructure;
using Accounts.Register.UserStory;

namespace Accounts.Register.WorkSteps;

internal sealed class Save(IAccountRepository repsoitory) {
    public async Task<bool> Run(Context context) {
        await repsoitory.CreateAccount(context.Account!, context.Token);
        return true;
    }
}
