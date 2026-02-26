namespace Features.Accounts.Slices.Register.Triggers.Blazor;

public interface IRegister {
    Task<RegisterResponse> Run(RegisterRequest blazorRequest, CancellationToken token = default);
}
