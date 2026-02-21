namespace Business.Features.Accounts.Infrastructure;

internal interface IRandom {
    void Generate(Span<byte> data);
}
