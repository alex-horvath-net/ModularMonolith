namespace Accounts.Login.WorkSteps;

internal sealed class Validate() {
    public bool Run(Context context) {

        if (context.Request.AccountType != AccountType.LocalAccount) {
            context.Response.ErrorMessage = UserStoryConstants.AccountTypeNotFound;
            return false;
        }

        if (!context.Request.Credentials.TryGetValue(UserStoryConstants.Email, out var email)) {
            context.Response.ErrorMessage = UserStoryConstants.MissingEmail;
            return false;
        }
        context.Email = email;

        if (!context.Request.Credentials.TryGetValue(UserStoryConstants.Password, out var password)) {
            context.Response.ErrorMessage = UserStoryConstants.MissingPassword;
            return false;
        }
        context.Password = password;

        return true;
    }
}
