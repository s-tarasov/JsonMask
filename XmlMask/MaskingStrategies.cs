using System;

namespace XmlMask
{
    public static class MaskingStrategies
    {
        public static XmlMasker.MaskValue Full { get; } = FullMasking;

        public static XmlMasker.MaskValue LastSymbolsByPercent(byte percent) =>
            (value) => LastSymbolsByPercentInternal(value, percent);

        private static void FullMasking(Span<char> value)
        {
            for (var pos = 0; pos < value.Length; pos++)
                value[pos] = '*';
        }

        private static void LastSymbolsByPercentInternal(Span<char> value, byte percent)
        {
            var previewLength = value.Length * percent / 100;
            for (var pos = previewLength; pos < value.Length; pos++)
                value[pos] = '*';
        }
    }
}
