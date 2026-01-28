using System.Security.Cryptography;
using System.Text;
using Experts.SecurityOfficer.Shared.Domain;
using Microsoft.EntityFrameworkCore;
using Data = Experts.SecurityOfficer.Shared.Infrastructure.Data;
using Domain = Experts.SecurityOfficer.Shared.Domain;

namespace Experts.SecurityOfficer.Login;

public class Authenticate(
    Authenticate.IStore store,
    Authenticate.IHasher hasher) {
    public async Task Run(
        UserStory.Request request,
        UserStory.Response response,
        CancellationToken token) {

        if (request.AccountType != UserStory.AccountType.LocalAccount) {
            response.ErrorMessage = "Account type not found";
            return;
        }

        await LocalAccountAuthentication(request, response, token);
        if (response.ErrorMessage!=null) {
            return;
        }

        return;
    }

    private async Task LocalAccountAuthentication(UserStory.Request request, UserStory.Response response, CancellationToken token) {
        if (!request.Credentials.TryGetValue("Email", out var email)) {
            response.ErrorMessage = "Credential not found. Missing Email";
            return;
        }

        if (!request.Credentials.TryGetValue("Password", out var password)) {
            response.ErrorMessage = "Credential not found. Missing Password";
            return;
        }

        var account = await store.FindByEmail(email, token);

        if (account is null) {
            response.ErrorMessage = "Account not found";
            return ;
        }

        if (account.IsLocked) {
            response.ErrorMessage = "Account locked";
            return ;
        }

        if (!hasher.Verify(password, account.PasswordHash)) {
            response.ErrorMessage = "Invalid password";
            return ;
        }

        response.AuthenticationId = account.Id;
    }

    public interface IStore {
        Task<Domain.Account> FindByEmail(string email, CancellationToken token);
    }

    public class Store(Data.SecurityOfficerDbContext db) : IStore {
        public async Task<Domain.Account> FindByEmail(string email, CancellationToken token) {
            Data.Models.Account? accountInfra =
                await db
                    .Accounts
                    .Where(account => account.Email == email)
                    .FirstOrDefaultAsync(token);

            Domain.Account accountDomain = MapToDomain(accountInfra);
            return accountDomain;
        }

        public Domain.Account MapToDomain(Data.Models.Account accountData) => new() {
            Id = accountData.Id,
            UserName = accountData.UserName,
            Email = accountData.Email,
            PasswordHash = accountData.PasswordHash,
            IsLocked = accountData.IsLocked,
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
                password: Encoding.UTF8.GetBytes(input),
                salt: salt,
                iterations: Iterations,
                hashAlgorithm: HashAlgorithmName.SHA256,
                outputLength: HashSize);

            // Combine salt and hash then encode to Base64 for storage
            byte[] hashBytes = new byte[SaltSize + HashSize];
            Buffer.BlockCopy(
                src: salt,
                srcOffset: 0,
                dst: hashBytes,
                dstOffset: 0,
                count: SaltSize);

            Buffer.BlockCopy(
                src: hash,
                srcOffset: 0,
                dst: hashBytes,
                dstOffset: SaltSize,
                count: HashSize);

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
                    password: Encoding.UTF8.GetBytes(input),
                    salt: storedSaltBytes,
                    iterations: Iterations,
                    hashAlgorithm: HashAlgorithmName.SHA256,
                    outputLength: HashSize);

                // Compare the hashes
                return CryptographicOperations.FixedTimeEquals(computedHashBytes, storedHashBytes);

            } catch {
                return false;
            }





        }
    }
}
