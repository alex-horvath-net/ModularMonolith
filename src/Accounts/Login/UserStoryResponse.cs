namespace Accounts.Login;

public sealed record UserStoryResponse() {
    public string? ErrorMessage { get; internal set; }
    public Guid? AuthenticationId { get; internal set; }
    public string? UserName { get; internal set; }
    public List<string> Roles { get; internal set; } = [];
}
