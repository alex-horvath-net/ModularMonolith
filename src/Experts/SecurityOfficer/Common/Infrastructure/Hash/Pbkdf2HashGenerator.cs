using System.Security.Cryptography;

namespace Experts.SecurityOfficer.Common.Infrastructure.Hash;

internal sealed class Pbkdf2HashGenerator(Random.IRandom random) : IHasher {
    private const int SaltSize = 16; // 128-bit salt
    private const int HashSize = 32; // 256-bit hash
    private const int Iterations = 100_000;

    public string Hash(string text) {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        Span<byte> salt = stackalloc byte[SaltSize];
        random.Fill(salt);

        var hashKey = Rfc2898DeriveBytes.Pbkdf2(
            password: text,
            salt: salt,
            iterations: Iterations,
            hashAlgorithm: HashAlgorithmName.SHA512,
            outputLength: HashSize);

        Span<byte> saltAndHashKey = stackalloc byte[SaltSize + HashSize];
        salt.CopyTo(saltAndHashKey);

        hashKey.CopyTo(saltAndHashKey[SaltSize..]);

        var hash = Convert.ToBase64String(saltAndHashKey);

        return hash;
    }

    public bool Verify(string text, string hash) {
        if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(hash))
            return false;

        byte[] saltAndHashKey;
        try {
            saltAndHashKey = Convert.FromBase64String(hash);
        } catch (FormatException) {
            return false;
        }

        if (saltAndHashKey.Length != SaltSize + HashSize)
            return false;

        Span<byte> salt = stackalloc byte[SaltSize];
        saltAndHashKey.AsSpan(0, SaltSize).CopyTo(salt);

        Span<byte> hashKey = stackalloc byte[HashSize];
        saltAndHashKey.AsSpan(SaltSize, HashSize).CopyTo(hashKey);

        var haskeyForText = Rfc2898DeriveBytes.Pbkdf2(
            password: text,
            salt: salt,
            iterations: Iterations,
            hashAlgorithm: HashAlgorithmName.SHA512,
            outputLength: HashSize);

        return CryptographicOperations.FixedTimeEquals(haskeyForText, hashKey);
    }
}
