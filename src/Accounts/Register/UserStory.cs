using Accounts.Core.Infrastructure;
using Core.Domain;
using Core.Infrastructure;

namespace Accounts.Register;

internal sealed class UserStory(IAccountRepository repository, IHasher hasher, IClock clock) : WorkStep<Context>(clock) {
    protected override async Task Run(Context context) {
        await validate.Execute(context);
        await normalize.Execute(context);
        await preventDuplication.Execute(context);
        await create.Execute(context);
        await save.Execute(context);
    }

    private readonly Validate validate = new(clock);
    private readonly Normalize normalize = new(clock);
    private readonly PreventDuplication preventDuplication = new(repository, clock);
    private readonly Create create = new(hasher, clock);
    private readonly Save save = new(repository, clock);
}