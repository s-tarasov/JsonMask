using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Utf8JsonMask;

public sealed class Utf8JsonMaskingPushStream : DelegatedStream
{
    private Utf8JsonMasker _masker;
    private static ReadOnlySpan<byte> Utf8Bom => new byte[] { 0xEF, 0xBB, 0xBF };

    public override bool CanRead => false;

    public override bool CanSeek => false;

    private bool _firstBytesProcceed;

    public Utf8JsonMaskingPushStream(Stream stream, Utf8JsonMasker masker) : base(stream)
    {
        _masker = masker;
    }

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        if (!_firstBytesProcceed)
        {
            if (buffer.StartsWith(Utf8Bom))
            {
                base.Write(Utf8Bom);
                buffer = buffer.Slice(Utf8Bom.Length);
            }

            _firstBytesProcceed = true;
        }

        _masker.Write(buffer, false);
        base.Write(_masker.MaskedBytes);
    }

    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        if (!_firstBytesProcceed)
        {
            if (buffer.Span.StartsWith(Utf8Bom))
            {
                base.Write(Utf8Bom);
                buffer = buffer.Slice(Utf8Bom.Length);
            }

            _firstBytesProcceed = true;
        }

        _masker.Write(buffer.Span, false);

        return base.WriteAsync(_masker.MaskedBytes, cancellationToken);
    }

    public override void Close()
    {
        _masker.Write(Array.Empty<byte>(), true);
        base.Write(_masker.MaskedBytes);
        base.Close();
    }
}

