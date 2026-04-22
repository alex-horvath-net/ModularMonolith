namespace Core.Infrastructure.Authentication.Basic;

public sealed record BasicCredentialValidationResult(bool IsAuthenticated, string UserName, IReadOnlyCollection<string> Roles) {
    public static BasicCredentialValidationResult Fail() => new(false, string.Empty, []);

    public static BasicCredentialValidationResult Success(string userName, IReadOnlyCollection<string> roles) => new(true, userName, roles);
}