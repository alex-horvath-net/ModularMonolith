using System.Security.Cryptography;

namespace Experts.SecurityOfficer.Shared.Security;

/// <summary>
/// Provides hashing and verification for credentials stored by the security officer expert.
/// </summary>
public interface IPasswordHasher {
    string Hash(string password);
    bool Verify(string password, string storedHash);
}

public sealed class Pbkdf2PasswordHasher : IPasswordHasher {
    private const int SaltSize = 16; // 128-bit salt
    private const int HashSize = 32; // 256-bit hash
    private const int Iterations = 100_000;

    public string Hash(string password) {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        Span<byte> salt = stackalloc byte[SaltSize];
        RandomNumberGenerator.Fill(salt);

        var derived = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA512,
            HashSize);

        Span<byte> payload = stackalloc byte[SaltSize + HashSize];
        salt.CopyTo(payload);
        derived.CopyTo(payload[SaltSize..]);

        return Convert.ToBase64String(payload);
    }

    public bool Verify(string password, string storedHash) {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(storedHash)) {
            return false;
        }

        byte[] payload;
        try {
            payload = Convert.FromBase64String(storedHash);
        } catch (FormatException) {
            return false;
        }

        if (payload.Length != SaltSize + HashSize) {
            return false;
        }

        Span<byte> salt = stackalloc byte[SaltSize];
        payload.AsSpan(0, SaltSize).CopyTo(salt);

        Span<byte> expectedHash = stackalloc byte[HashSize];
        payload.AsSpan(SaltSize, HashSize).CopyTo(expectedHash);

        var computed = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA512,
            HashSize);

        return CryptographicOperations.FixedTimeEquals(computed, expectedHash);
    }
}
