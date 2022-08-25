using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Utf8JsonMask;

public sealed class Utf8JsonMaskingPullStream : DelegatedStream
{
    private Utf8JsonMasker _masker;
    private static ReadOnlySpan<byte> Utf8Bom => new byte[] { 0xEF, 0xBB, 0xBF };
    private int _consumedMaskedByteCount;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    private bool _firstBytesProcceed;

    public Utf8JsonMaskingPullStream(Stream stream, Utf8JsonMasker masker) : base(stream)
    {
       _masker = masker;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        var resultBytesCount = 0;
        var bufferSpan = buffer.AsSpan(offset);
        while (resultBytesCount < count)
        {
            var availableMaskedBytes = _masker.MaskedBytes.AsSpan(_consumedMaskedByteCount);
            if (availableMaskedBytes.Length > 0)
            {
                var requireForConsuming = count - resultBytesCount;
                var consumedMaskedBytesCount = requireForConsuming < availableMaskedBytes.Length
                    ? requireForConsuming
                    : availableMaskedBytes.Length;
                availableMaskedBytes.Slice(0, consumedMaskedBytesCount).CopyTo(bufferSpan);
                bufferSpan = bufferSpan.Slice(consumedMaskedBytesCount);
                resultBytesCount += consumedMaskedBytesCount;
                _consumedMaskedByteCount += consumedMaskedBytesCount;
            }
            else
            {
                _consumedMaskedByteCount = 0;

                var inputBuffer = _masker.InputBuffer;
                var bytesCount = base.Read(inputBuffer.Array, inputBuffer.Offset, inputBuffer.Count);
                if (!_firstBytesProcceed)
                {
                    var inputBuffeSpan = inputBuffer.AsSpan();
                    if (inputBuffeSpan.StartsWith(Utf8Bom))
                    {
                        resultBytesCount += Utf8Bom.Length;
                        bytesCount -= Utf8Bom.Length;
                        inputBuffeSpan.Slice(Utf8Bom.Length, bytesCount).CopyTo(inputBuffeSpan);
                        Utf8Bom.CopyTo(bufferSpan);
                        bufferSpan = bufferSpan.Slice(Utf8Bom.Length);
                    }

                    _firstBytesProcceed = true;
                }
                var final = bytesCount == 0;
                _masker.WriteFromInputBuffer(bytesCount, final);
                if (final)
                    return resultBytesCount;
            }
        }

        return resultBytesCount;
    }
}

