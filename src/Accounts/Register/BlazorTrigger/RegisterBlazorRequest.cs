namespace Accounts.Register.BlazorTrigger;

public sealed class RegisterBlazorRequest {
    public DateTime? RunTime { get; set; }
    public Guid? RunId { get; set; }
    public Guid CorrelationId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public List<string> Roles { get; } = [];
}
