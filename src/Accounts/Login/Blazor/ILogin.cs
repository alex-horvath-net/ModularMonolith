namespace Accounts.Login.Blazor;

public interface ILogin {
    Task<LoginResponse> Run(LoginRequest clientRequest, CancellationToken token = default);
}
