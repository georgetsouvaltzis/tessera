using BenchmarkDotNet.Attributes;
using Tessera.Hosting;

namespace Tessera.Benchmarks;

[MemoryDiagnoser]
public class InputDecodingBenchmarks
{
    private static readonly byte[] OscClipboardSequence =
    [
        0x1B, (byte)']', (byte)'5', (byte)'2', (byte)';', (byte)'c', (byte)';',
        (byte)'a', (byte)'G', (byte)'V', (byte)'s', (byte)'b', (byte)'G', (byte)'8', (byte)'=', 0x07,
    ];

    private static readonly byte[] OscColorSequence =
    [
        0x1B, (byte)']', (byte)'1', (byte)'0', (byte)';', (byte)'r', (byte)'g', (byte)'b', (byte)':',
        (byte)'a', (byte)'a', (byte)'/', (byte)'b', (byte)'b', (byte)'/', (byte)'c', (byte)'c', 0x1B, (byte)'\\',
    ];

    private static readonly byte[] DcsCapabilitySequence =
    [
        0x1B, (byte)'P', (byte)'1', (byte)'+', (byte)'r',
        (byte)'5', (byte)'4', (byte)'6', (byte)'3', (byte)'=',
        (byte)'3', (byte)'1', 0x1B, (byte)'\\',
    ];

    private readonly EventDecoder _decoder = new();

    [Benchmark(Description = "decode osc clipboard")]
    public int DecodeOscClipboard()
    {
        var result = _decoder.Decode(OscClipboardSequence, timeoutExpired: false);
        return result.Consumed;
    }

    [Benchmark(Description = "decode osc color")]
    public int DecodeOscColor()
    {
        var result = _decoder.Decode(OscColorSequence, timeoutExpired: false);
        return result.Consumed;
    }

    [Benchmark(Description = "decode dcs capability")]
    public int DecodeDcsCapability()
    {
        var result = _decoder.Decode(DcsCapabilitySequence, timeoutExpired: false);
        return result.Consumed;
    }
}
