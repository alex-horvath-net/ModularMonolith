using Business.Features.Accounts.Infrastructure;
using Business.Features.Accounts.Slices.Register;

namespace Business.Features.Accounts.Slices.Register.WorkSteps;
internal class Save(IAccountRepository repsoitory) {
    public async Task<bool> Run(UserStory.UserStoryContext context) {

        await repsoitory.CreateAccount(context.Account!, context.Token);

        return true;
    }
}
