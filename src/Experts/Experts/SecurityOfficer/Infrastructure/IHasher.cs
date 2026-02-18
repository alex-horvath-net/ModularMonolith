namespace Business.Experts.SecurityOfficer.Infrastructure;

internal interface IHasher {
    string Generate(string text);
    bool Verify(string text, string hash);
}