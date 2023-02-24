using System.Buffers;

namespace Utf8JsonMask.Tests;

public class Utf8JsonMaskingPullStreamTests
{
    private Utf8JsonMasker _masker;
    private FieldUtf8Selector _fieldSelector = bytes 
        => "key"u8.SequenceEqual(bytes) ? Utf8MaskingStrategies.Full : null;

    public Utf8JsonMaskingPullStreamTests()
    {
        _masker = new(f => _fieldSelector(f), ArrayPool<byte>.Shared);
    }

    [Fact]
    public void LargeFileStream()
    {
        var inputString = "[" + string.Join(",", Enumerable.Repeat("""
                {
                    "key": "secret123"
                }
                """, 100000)) + "]";
        string path = Path.GetTempFileName();
        File.WriteAllText(path, inputString);
        FileStream inputStream = File.OpenRead(path);

        var outputString = new StreamReader(new Utf8JsonMaskingPullStream(inputStream, _masker), Encoding.UTF8).ReadToEnd();

        Assert.Equal("[" + string.Join(",", Enumerable.Repeat("""
                {
                    "key": "*********"
                }
                """, 100000)) + "]", outputString);
    }

    [Fact]
    public void MemoryStream()
    {
        var inputStream = ToMemoryStream("""
                {
                    "key": "secret123"
                }
                """);

        var outputString = new StreamReader(new Utf8JsonMaskingPullStream(inputStream, _masker), Encoding.UTF8).ReadToEnd();

        Assert.Equal("""
                {
                    "key": "*********"
                }
                """, outputString);
    }

    private static MemoryStream ToMemoryStream(string inputString)
    {
        var stream = new MemoryStream();
        using (var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true))
        {
            writer.Write(inputString);
        }
        stream.Position = 0;
        return stream;
    }
}
