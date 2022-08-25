namespace Utf8JsonMask.Tests;

public class Utf8JsonMaskerTests
{
    private Utf8JsonMasker _sut;
    private FieldSelector _fieldSelector = b => "secretKey"u8.SequenceEqual(b);

    public Utf8JsonMaskerTests()
    {
        _sut = new(f => _fieldSelector(f));
    }

    [Fact]
    public void TestMaskValue()
    {
        var utf8Json = """
                {
                    "key": "value1",
                    "secretKey": "value2",
                    "anotherKey": "value3"
                }
                """u8;

        _sut.Write(utf8Json, true);

        AssertEqual("""
                {
                    "key": "value1",
                    "secretKey": "******",
                    "anotherKey": "value3"
                }
                """u8, _sut.MaskedBytes);
    }

    [Theory]
    [InlineData(5)]
    [InlineData(15)]
    [InlineData(25)]
    [InlineData(35)]
    [InlineData(45)]
    [InlineData(52)]
    public void TestMaskValueInTwoBuffers(int splitPosition)
    {
        var utf8Json = """
                {
                    "key1": "secret123",
                    "key2": "secret1"
                }
                """u8;
        _fieldSelector = b => b.StartsWith("key"u8);
        var maskedJsonUtf8 = new MemoryStream();

        _sut.Write(utf8Json.Slice(0, splitPosition), false);
        maskedJsonUtf8.Write(_sut.MaskedBytes);

        _sut.Write(utf8Json.Slice(splitPosition), true);
        maskedJsonUtf8.Write(_sut.MaskedBytes);

        AssertEqual("""
                {
                    "key1": "*********",
                    "key2": "*******"
                }
                """u8, maskedJsonUtf8.ToArray());
    }

    [Fact]
    public void TestMaskNestedValue()
    {
        var utf8Json = """
                {
                    "key1": "secret123",
                    "key2": 
                     {
                        "secretKey": "111"
                     },
                    "key3": "key3value"
                }
                """u8;

        _sut.Write(utf8Json, true);

        AssertEqual("""
                {
                    "key1": "secret123",
                    "key2": 
                     {
                        "secretKey": "***"
                     },
                    "key3": "key3value"
                }
                """u8, _sut.MaskedBytes);
    }

    private static void AssertEqual(ReadOnlySpan<byte> expected, ReadOnlySpan<byte> actual)
    {
        if (actual.SequenceEqual(expected))
            return;

        Assert.Equal(
            Encoding.UTF8.GetString(expected.ToArray()),
            Encoding.UTF8.GetString(actual.ToArray()));
    }
}
