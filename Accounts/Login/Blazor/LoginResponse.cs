namespace Accounts.Login.Blazor;

public record LoginResponse(
    string? ErrorMessage,
    Guid? AuthenticationId,
    string? UserName,
    List<string> Roles);
