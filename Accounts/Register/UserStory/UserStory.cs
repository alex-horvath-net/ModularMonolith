using Accounts.Core.Infrastructure;
using Accounts.Register.WorkSteps;
using Core.Infrastructure;

namespace Accounts.Register.UserStory;

internal sealed class UserStory(
    IAccountRepository repository,
    IHasher hasher,
    IClock clock,
    ILogger<UserStory> logger) {

    private readonly Validate validate = new(clock, logger);
    private readonly Normalize normalize = new(clock, logger);
    private readonly PreventDuplication preventDuplication = new(repository, clock, logger);
    private readonly Create create = new(hasher, clock, logger);
    private readonly Save save = new(repository, clock, logger);

    internal async Task<Product<Response>> Register(
        Request request,
        CancellationToken token) {

        var context = new Context(request, token);

        try {
            await validate.Execute(context);
            await normalize.Execute(context);
            await preventDuplication.Execute(context);
            await create.Execute(context);
            await save.Execute(context);

            return Product<Response>.FromContext(context, context.ToResponse());
        } catch (InvalidOperationException exception) {
            exception.Data[$"{typeof(Product<Response>).FullName}.{nameof(Product<>.ExecutedBusinessWorkSteps)}"] = context.ExecutedBusinessWorkSteps.ToArray();
            throw;
        }
    }
}
