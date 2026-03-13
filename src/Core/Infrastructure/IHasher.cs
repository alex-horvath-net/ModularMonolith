namespace Core.Infrastructure;

public interface IHasher {
    string Generate(string text);
    bool Verify(string text, string hash);
}