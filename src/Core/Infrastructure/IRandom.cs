namespace Core.Infrastructure;

internal interface IRandom {
    void Generate(Span<byte> data);
}
