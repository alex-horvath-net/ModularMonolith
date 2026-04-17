namespace Accounts.Register.BlazorTrigger;

public interface IRegisterAdapter {
    Task<RegisterBlazorResponse> Run(RegisterBlazorRequest blazorRequest, CancellationToken token = default);
}
