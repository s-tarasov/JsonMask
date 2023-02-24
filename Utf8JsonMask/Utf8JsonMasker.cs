using System;
using System.Buffers;
using System.Text.Json;

namespace Utf8JsonMask;

/// TODO Make internal
public struct Utf8JsonMasker : IDisposable
{
    // Buffer structure mmmmmmmppppppppppiiiiiiiiiiiiiiiiiiiii
    // mmmmmmm - masked segment [0, _availableCount]
    // pppppppppp - notmasked segment [_availableCount, _size]
    // iiiiiiiiiiiiiiiiiiiii - input buffer segment [_size, ...]
    private byte[] _buffer;

    private int _availableCount;
    private int _size;
    private JsonReaderState _jsonReaderState;
    private readonly FieldUtf8Selector _fieldSelector;
    private readonly ArrayPool<byte> _bytesPool;
    private MaskUtf8Value? _maskNextToken;

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

    public Utf8JsonMasker(FieldUtf8Selector fieldSelector, ArrayPool<byte> bytesPool)
    {
        _fieldSelector = fieldSelector;
        _bytesPool = bytesPool;
    }

    internal void EnsureInputBufferCapacity(int min) => EnsureBufferCapacity(_size + min);

    private void EnsureBufferCapacity(int min)
    {
        if (_buffer == null)
        {
            _buffer = _bytesPool.Rent(min);
        }
        else if (_buffer.Length < min)
        {
            var newBuffer = _bytesPool.Rent(min);
            _buffer.AsSpan().CopyTo(newBuffer);
            _bytesPool.Return(_buffer);
            _buffer = newBuffer;
        }
    }

    internal void Write(ReadOnlySpan<byte> stringBytesIn, bool final)
    {
        EnsureInputBufferCapacity(stringBytesIn.Length);
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
                case JsonTokenType.String when (_maskNextToken is not null):
                    {
                        Span<byte> valueSpan = _buffer.AsSpan((int)jsonReader.TokenStartIndex + 1, jsonReader.ValueSpan.Length);
                        _maskNextToken(valueSpan);
                        break;
                    }
            }
        }
        _jsonReaderState = jsonReader.CurrentState;
        _availableCount += (int)jsonReader.BytesConsumed;
    }

    public void Dispose()
    {
        if (_buffer != null)
            _bytesPool.Return(_buffer);
    }
}
