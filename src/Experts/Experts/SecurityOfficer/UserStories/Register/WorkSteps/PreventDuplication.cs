using Business.Experts.SecurityOfficer.Domain;

namespace Business.Experts.SecurityOfficer.UserStories.Register.WorkSteps;
internal class PreventDuplication(PreventDuplication.IRepository repository) {
    public async Task<bool> Run(UserStory.UserStoryContext context) {

        context.MathingAccount = await repository.FindAccountByEmail(context.NormalizedRequest!.Email, context.Token);
        if (context.MathingAccount is not null) {
            context.Response.ErrorMessage = UserStoryConstants.AccountAlreadyExists;
            return false;
        }

        return true;
    }

    internal interface IRepository {
        Task<Account?> FindAccountByEmail(string email, CancellationToken token);
    }
}