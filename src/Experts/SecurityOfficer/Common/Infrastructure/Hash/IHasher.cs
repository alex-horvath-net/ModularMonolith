namespace Experts.SecurityOfficer.Common.Infrastructure.Hash;

internal interface IHasher {
    string Hash(string text);
    bool Verify(string text, string hash);
}