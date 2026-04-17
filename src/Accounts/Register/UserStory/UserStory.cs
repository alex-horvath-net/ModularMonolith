using Accounts.Core.Infrastructure;
using Core.Domain;
using Core.Infrastructure;

namespace Accounts.Register.UserStory;

internal sealed class UserStory(
    IAccountRepository repository,
    IHasher hasher,
    IClock clock,
    IGuidGenerator guid,
    ILogger<UserStory> logger) : WorkStep<Context>(clock, guid, logger) {

    protected override async Task Run(Context context) {
        await validate.Execute(context);
        await normalize.Execute(context);
        await preventDuplication.Execute(context);
        await create.Execute(context);
        await save.Execute(context);
    }

    private readonly Validate validate = new(clock, guid, logger);
    private readonly Normalize normalize = new(clock, guid, logger);
    private readonly PreventDuplication preventDuplication = new(repository, clock, guid, logger);
    private readonly Create create = new(hasher, clock, guid, logger);
    private readonly Save save = new(repository, clock, guid, logger);
}