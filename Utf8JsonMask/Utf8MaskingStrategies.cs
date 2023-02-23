using System;

namespace Utf8JsonMask;

public static class Utf8MaskingStrategies
{
    public static readonly byte MaskSymbol = "*"u8[0];

    public static MaskUtf8Value Full { get; } = FullMasking;

    /// <summary> 1234567890 -> 123******* </summary>
    /// <param name="percent">Percent of symbols should be masked</param>
    public static MaskUtf8Value LastSymbolsByPercent(byte percent) =>
        (value) => LastSymbolsByPercentInternal(value, percent);

    private static void FullMasking(Span<byte> value)
    {
        for (var pos = 0; pos < value.Length; pos++)
            value[pos] = MaskSymbol;
    }
    
    private static void LastSymbolsByPercentInternal(Span<byte> value, byte percent)
    {
        var maskedLength = value.Length * percent / 100;
        for (var pos = value.Length - maskedLength; pos < value.Length; pos++)
            value[pos] = MaskSymbol;
    }
}
