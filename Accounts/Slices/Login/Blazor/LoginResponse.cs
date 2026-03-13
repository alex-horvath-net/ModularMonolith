namespace Accounts.Slices.Login.Blazor;

public record LoginResponse(
    string? ErrorMessage,
    Guid? AuthenticationId,
    string? UserName,
    List<string> Roles);
