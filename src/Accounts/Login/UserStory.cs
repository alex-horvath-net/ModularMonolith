using Accounts.Core.Infrastructure;
using Accounts.Login.WorkSteps;
using Core.Infrastructure;

namespace Accounts.Login;

internal sealed class UserStory {
    private readonly Validate validate;
    private readonly Normalize normalize;
    private readonly Authenticate authenticate;
    private readonly Authorize authorize;

    internal UserStory(IAccountRepository repository, IHasher hasher) {
        validate = new Validate();
        normalize = new Normalize();
        authenticate = new Authenticate(repository, hasher);
        authorize = new Authorize();
    }

    public async Task<UserStoryResponse> Run(UserStoryRequest request, CancellationToken token) {
        var context = new Context(request, new(), token);

        if (!validate.Run(context))
            return context.Response;

        if (!normalize.Run(context))
            return context.Response;

        if (!await authenticate.Run(context))
            return context.Response;

        if (!await authorize.Run(context))
            return context.Response;

        return context.Response;
    }
}