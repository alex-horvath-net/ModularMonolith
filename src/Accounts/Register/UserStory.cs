using Accounts.Core.Infrastructure;
using Core.Domain;
using Core.Infrastructure;

namespace Accounts.Register;

internal sealed class UserStory(
    IAccountRepository repository,
    IHasher hasher,
    IClock clock,
    IGuidGenerator guid) : WorkStep<Context>(clock, guid) {

    protected override async Task Run(Context context) {
        await validate.Execute(context);
        await normalize.Execute(context);
        await preventDuplication.Execute(context);
        await create.Execute(context);
        await save.Execute(context);
    }

    private readonly Validate validate = new(clock, guid);
    private readonly Normalize normalize = new(clock, guid);
    private readonly PreventDuplication preventDuplication = new(repository, clock, guid);
    private readonly Create create = new(hasher, clock, guid);
    private readonly Save save = new(repository, clock, guid);
}