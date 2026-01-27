using Microsoft.EntityFrameworkCore;
using Domain = Experts.SecurityOfficer.Shared.Domain;
using Data = Experts.SecurityOfficer.Shared.Infrastructure.Data;
using System.Security.Cryptography;
using System.Text;

namespace Experts.SecurityOfficer.Login;

public class Authenticate(
    Authenticate.IStore store,
    Authenticate.IHasher hasher) {
    public async Task<Domain.Account?> Run(UserStory.AccountType accountType, IReadOnlyDictionary<string, string> credentials, CancellationToken token) {
        if (accountType != UserStory.AccountType.LocalAccount)
            return null;

        if (!credentials.TryGetValue("Email", out var email))
            return null;

        if (!credentials.TryGetValue("Password", out var password)) {
            return null;
        }

        var account = await store.FindByEmail(email, token);

        if (account is null)
            return null;

        if (account.IsLocked)
            return null;

        if (!hasher.Verify(password, account.PasswordHash))
            return null;

        return account;
    }

    public interface IStore {
        Task<Domain.Account> FindByEmail(string email, CancellationToken token);
    }

    public class Store(Data.SecurityOfficerDbContext db) : IStore {
        public async Task<Domain.Account> FindByEmail(string email, CancellationToken token) {
            Data.Models.Account accountData = await db.Accounts.Where(account => account.Email == email).SingleAsync(token);
            Domain.Account accountDomain = MapToDomain(accountData);
            return accountDomain;
        }

        public Domain.Account MapToDomain(Data.Models.Account accountData) => new() {
            IsLocked = accountData.IsLocked,
            Email = accountData.Email,
            PasswordHash = accountData.PasswordHash
        };
    }


    public interface IHasher {
        string Hash(string input);
        bool Verify(string input, string storedHashedInput);
    }

    public class BCryptHasher : IHasher {
        public string Hash(string input) {
            return BCrypt.Net.BCrypt.HashPassword(input);
        }

        public bool Verify(string input, string storedHashedInput) {
            try {
                return BCrypt.Net.BCrypt.Verify(input, storedHashedInput);
            } catch {
                return false;
            }
        }
    }

    public class Pbkdf2Hasher : IHasher {
        private const int SaltSize = 16; // 128 bit
        private const int HashSize = 32; // 256 bit
        private const int Iterations = 10000; // PBKDF2 iterations count

        public string Hash(string input) {
            if (string.IsNullOrEmpty(input)) {
                throw new ArgumentException("Input cannot be null or empty", nameof(input));
            }

            // Generate a random salt
            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create()) {
                rng.GetBytes(salt);
            }

            // Hash the input with the salt using PBKDF2
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(input),
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                HashSize);

            // Combine salt and hash then encode to Base64 for storage
            byte[] hashBytes = new byte[SaltSize + HashSize];
            Buffer.BlockCopy(salt, 0, hashBytes, 0, SaltSize);
            Buffer.BlockCopy(hash, 0, hashBytes, SaltSize, HashSize);
            return Convert.ToBase64String(hashBytes);
        }

        public bool Verify(string input, string storedHashedInput) {
            if (string.IsNullOrEmpty(input)) {
                return false;
            }

            if (string.IsNullOrEmpty(storedHashedInput)) {
                return false;
            }


            try {
                // Decode the hashediInput
                byte[] storedHashedInputBytes = Convert.FromBase64String(storedHashedInput);
                if (storedHashedInputBytes.Length != SaltSize + HashSize) {
                    return false;
                }

                // Extract the stored salt. 
                byte[] storedSaltBytes = new byte[SaltSize];
                Array.Copy(
                    sourceArray: storedHashedInputBytes,
                    sourceIndex: 0,
                    destinationArray: storedSaltBytes,
                    destinationIndex: 0,
                    length: SaltSize);

                // Extract the stored hash. 
                byte[] storedHashBytes = new byte[HashSize];
                Array.Copy(
                    sourceArray: storedHashedInputBytes,
                    sourceIndex: SaltSize,
                    destinationArray: storedHashBytes,
                    destinationIndex: 0,
                    length: HashSize);

                // Hash the provided input with the extracted salt using PBKDF2
                byte[] computedHashBytes = Rfc2898DeriveBytes.Pbkdf2(
                    Encoding.UTF8.GetBytes(input),
                    storedSaltBytes,
                    Iterations,
                    HashAlgorithmName.SHA256,
                    HashSize);

                // Compare the hashes
                return CryptographicOperations.FixedTimeEquals(computedHashBytes, storedHashBytes);

            } catch {
                return false;
            }





        }
    }
}
