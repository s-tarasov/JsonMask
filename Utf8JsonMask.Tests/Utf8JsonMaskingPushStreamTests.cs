namespace Utf8JsonMask.Tests;

public class Utf8JsonMaskingPushStreamTests
{
    private Utf8JsonMasker _masker;
    private FieldSelector _fieldSelector = b => "key"u8.SequenceEqual(b);

    public Utf8JsonMaskingPushStreamTests()
    {
        _masker = new(f => _fieldSelector(f));
    }

    [Fact]
    public void MemoryStream()
    {
        var inputString = """
                {
                    "key": "secret123"
                }
                """;

        var resultStream = new MemoryStream();
        var streamWriter = new StreamWriter(new Utf8JsonMaskingPushStream(resultStream, _masker), Encoding.UTF8, leaveOpen: true);
        streamWriter.Write(inputString);
        streamWriter.Close();

        resultStream.Position = 0;
        var outputString = new StreamReader(resultStream).ReadToEnd();

        Assert.Equal("""
                {
                    "key": "*********"
                }
                """, outputString);
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
        var resultStream = File.OpenWrite(path);

        using (var streamWriter = new StreamWriter(new Utf8JsonMaskingPushStream(resultStream, _masker), Encoding.UTF8))
        {
            streamWriter.Write(inputString);
        }

        var outputString = File.ReadAllText(path);
        Assert.Equal("[" + string.Join(",", Enumerable.Repeat("""
                {
                    "key": "*********"
                }
                """, 100000)) + "]", outputString);
    }
}
