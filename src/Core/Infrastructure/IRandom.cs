namespace Core.Infrastructure;

public interface IRandom {
    void Generate(Span<byte> data);
}
