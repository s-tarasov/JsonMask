using System.Text;

namespace JsonMask
{
    public static class MaskingStrategies
    {
        public static JsonMasker.MaskValue Full { get; } = FullMasking;

        public static JsonMasker.MaskValue LastSymbolsByPercent(byte percent) =>
            (sb, stP, enP) => LastSymbolsByPercentInternal(sb, stP, enP, percent);

        private static void FullMasking(StringBuilder stringBuilder, int startPosition, int endPosition)
        {
            for (var pos = startPosition; pos <= endPosition; pos++)
                stringBuilder[pos] = '*';
        }

        private static void LastSymbolsByPercentInternal(StringBuilder stringBuilder,
            int startPosition, int endPosition, byte percent)
        {
            var valueLength = 1 + endPosition - startPosition;
            var previewLength = valueLength * percent / 100;
            for (var pos = startPosition + previewLength; pos <= endPosition; pos++)
                stringBuilder[pos] = '*';
        }
    }
}
