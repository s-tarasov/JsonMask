using System;

namespace Utf8JsonMask;

/// <param name="fieldNameBytes">Unencoded bytes of json field name</param>
/// <returns>
/// null - dont mask field value.
/// MaskUtf8Value - mask field value using this strategy
/// </returns>
public delegate MaskUtf8Value? FieldUtf8Selector(ReadOnlySpan<byte> fieldNameBytes);
