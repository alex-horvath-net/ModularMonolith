using Business.Experts.SecurityOfficer.Domain;

namespace Business.Experts.SecurityOfficer.UserStories.Register.WorkSteps;
internal class Save(Save.IRepository repsoitory) {
    public async Task<bool> Run(UserStory.UserStoryContext context) {

        await repsoitory.CreateAccount(context.Account!, context.Token);

        return true;
    }

    internal interface IRepository {
        Task CreateAccount(Account account, CancellationToken token);
    }
}
