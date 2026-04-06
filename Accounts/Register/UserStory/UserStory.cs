using Accounts.Core.Infrastructure;
using Accounts.Register.WorkSteps;
using Core.Infrastructure;

namespace Accounts.Register.UserStory;

internal sealed class UserStory {
    internal async Task<Product<Response>> Register(Request request, CancellationToken token) {
        var context = new Context(request, token);

        try {
            await validate.Run(context);

            await normalize.Run(context);

            await preventDuplication.Run(context);

            await create.Run(context);

            await save.Run(context);

            //Activate email
            //Activate MFA

            return Product<Response>.FromContext(context, context.ToResponse());
        } catch (InvalidOperationException exception) {
            exception.Data[$"{typeof(Product<Response>).FullName}.{nameof(Product<>.ExecutedBusinessWorkSteps)}"] = context.ExecutedBusinessWorkSteps.ToArray();
            throw;
        }
    }

    internal UserStory(IAccountRepository repository, IHasher hasher, IClock clock) {
        validate = new Validate();
        normalize = new Normalize();
        preventDuplication = new PreventDuplication(repository);
        create = new Create(hasher, clock);
        save = new Save(repository);
    }

    private readonly Validate validate;
    private readonly Normalize normalize;
    private readonly PreventDuplication preventDuplication;
    private readonly Create create;
    private readonly Save save;
}
