namespace Core.Infrastructure.Authentication.Basic;

public sealed class BasicCredentialValidator : IBasicCredentialValidator {
    // Demo only.
    // In production: do NOT keep raw passwords in memory/config like this.
    // Validate against a database / external identity store / password hash.
    private static readonly Dictionary<string, UserRecord> Users =
        new(StringComparer.Ordinal) {
            ["alex"] = new UserRecord(
                Password: "P@ssw0rd!",
                Roles: ["Admin", "User"]),

            ["reader"] = new UserRecord(
                Password: "ReadOnly123!",
                Roles: ["User"])
        };

    public Task<BasicCredentialValidationResult> ValidateAsync(
        string userName,
        string password,
        CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();

        if (!Users.TryGetValue(userName, out var user)) {
            return Task.FromResult(BasicCredentialValidationResult.Fail());
        }

        // Demo comparison only.
        // In production compare password hashes, not plaintext values.
        var passwordMatches = string.Equals(
            user.Password,
            password,
            StringComparison.Ordinal);

        if (!passwordMatches) {
            return Task.FromResult(BasicCredentialValidationResult.Fail());
        }

        return Task.FromResult(
            BasicCredentialValidationResult.Success(userName, user.Roles));
    }

    private sealed record UserRecord(
        string Password,
        IReadOnlyCollection<string> Roles);
}