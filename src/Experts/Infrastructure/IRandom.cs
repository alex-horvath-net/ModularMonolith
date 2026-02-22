namespace Business.Infrastructure;

internal interface IRandom {
    void Generate(Span<byte> data);
}
