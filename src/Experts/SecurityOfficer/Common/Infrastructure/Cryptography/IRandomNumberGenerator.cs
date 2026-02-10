namespace Experts.SecurityOfficer.Common.Infrastructure.Cryptography;
internal interface IRandomNumberGenerator {
    void Fill(Span<byte> data);
}
