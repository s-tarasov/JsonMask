using System;

namespace Utf8JsonMask
{
    public delegate bool FieldSelector(ReadOnlySpan<byte> filedNameBytes);
}
