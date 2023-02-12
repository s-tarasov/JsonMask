using System;
using System.Text.Json;

namespace Utf8JsonMask;

public struct Utf8JsonMasker
{
    // Buffer structure mmmmmmmppppppppppiiiiiiiiiiiiiiiiiiiii
    // mmmmmmm - masked segment [0, _availableCount]
    // pppppppppp - notmasked segment [_availableCount, _size]
    // iiiiiiiiiiiiiiiiiiiii - input buffer segment [_size, ...]
    private byte[] _buffer = new byte[100000];

    private int _availableCount;
    private int _size;
    private JsonReaderState _jsonReaderState;
    private readonly FieldSelector _fieldSelector;
    private bool _maskNextToken;

    public ArraySegment<byte> MaskedBytes => new ArraySegment<byte>(_buffer, 0, _availableCount);

    public ArraySegment<byte> InputBuffer
    {
        get
        {
            if (_availableCount > 0)
            {
                _size -= _availableCount;
                Array.Copy(_buffer, _availableCount, _buffer, 0, _size);
                _availableCount = 0;
            }

            return new(_buffer, _size, _buffer.Length - _size);
        }
    }

    public Utf8JsonMasker(FieldSelector fieldSelector)
    {
        _fieldSelector = fieldSelector;
    }

    internal void Write(ReadOnlySpan<byte> stringBytesIn, bool final)
    {
        stringBytesIn.CopyTo(InputBuffer);
        WriteFromInputBuffer(stringBytesIn.Length, final);
    }

    internal void WriteFromInputBuffer(int inputBufferBytesCount, bool final)
    {
        _size += inputBufferBytesCount;

        var jsonReader = new Utf8JsonReader(_buffer.AsSpan(0, _size), final, _jsonReaderState);
        while (jsonReader.Read())
        {
            switch (jsonReader.TokenType)
            {
                case JsonTokenType.PropertyName:
                    _maskNextToken = _fieldSelector(jsonReader.ValueSpan);
                    break;
                case JsonTokenType.String when (_maskNextToken):
                    {
                        Span<byte> valueSpan = _buffer.AsSpan((int)jsonReader.TokenStartIndex + 1, jsonReader.ValueSpan.Length);
                        for (int i = 0; i < valueSpan.Length; i++)
                        {
                            valueSpan[i] = "*"u8[0];
                        }

                        break;
                    }
            }
        }
        _jsonReaderState = jsonReader.CurrentState;
        _availableCount += (int)jsonReader.BytesConsumed;
    }
}
