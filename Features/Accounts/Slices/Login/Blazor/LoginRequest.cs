using Features.Accounts.Domain;

namespace Features.Accounts.Slices.Login.Blazor;

public class LoginRequest(ApplicationUser applicationUser) {
    public ApplicationUser ApplicationUser { get; } = applicationUser ?? throw new ArgumentNullException(nameof(applicationUser));

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}
