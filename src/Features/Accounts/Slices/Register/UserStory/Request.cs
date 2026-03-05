namespace Features.Accounts.Slices.Register.UserStory;

internal sealed record Request(
    string Email,
    string UserName,
    string Password,
    IReadOnlyCollection<string> Roles);
