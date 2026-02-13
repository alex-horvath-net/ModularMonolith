using System.Security.Cryptography;

namespace Experts.SecurityOfficer.Common.Infrastructure.Random;

internal sealed class RandomGenerator : IRandom {
    public void Fill(Span<byte> data) => RandomNumberGenerator.Fill(data);
}

