namespace Core.Infrastructure.Authentication.Basic;

public interface IBasicCredentialValidator {
    Task<BasicCredentialValidationResult> ValidateAsync(
        string userName,
        string password,
        CancellationToken cancellationToken);
}

// "Realm": "Trading API" ?
