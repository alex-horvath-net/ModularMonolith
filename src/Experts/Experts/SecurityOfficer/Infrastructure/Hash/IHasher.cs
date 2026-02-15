namespace Business.Experts.SecurityOfficer.Infrastructure.Hash;

internal interface IHasher {
    string Generate(string text);
    bool Verify(string text, string hash);
}