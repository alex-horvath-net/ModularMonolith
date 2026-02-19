namespace Business.Features.Accounts.Infrastructure;

internal interface IHasher {
    string Generate(string text);
    bool Verify(string text, string hash);
}