namespace Accounts.Register.Triggers.Blazor;

public sealed class RegisterRequest {
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public IList<string> Roles { get; } = [];

    public void ReplaceRoles(IEnumerable<string> roles) {
        ArgumentNullException.ThrowIfNull(roles);

        Roles.Clear();
        foreach (var role in roles)
            if (!string.IsNullOrWhiteSpace(role))
                Roles.Add(role);
    }
}
