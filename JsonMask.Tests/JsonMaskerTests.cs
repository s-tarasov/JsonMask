using Xunit;

namespace JsonMask.Tests
{
    public class JsonMaskerTests
    {
        private readonly JsonMasker _masker = new JsonMasker();

        [Fact]
        public void MaskByPropertName()
        {
            var json =
@"{
   ""code"":  ""code"",
  ""test"": ""test"",
}";

            var maskedJson = _masker.MaskByPropertyName(json, "code", "fake");

            Assert.Equal(
@"{
   ""code"":  ""****"",
  ""test"": ""test"",
}", maskedJson);
        }

        [Fact]
        public void MaskByPath()
        {
            var json = @"{
   ""code"" :  ""code"",
  ""test"":""test"",
    ""i"": {
         ""code"": ""code2"",
    } 
}";

            var maskedJson = _masker.Mask(json, p => p.Name == "code" && p.GetPath() == "i.code");

            Assert.Equal(@"{
   ""code"" :  ""code"",
  ""test"":""test"",
    ""i"": {
         ""code"": ""*****"",
    } 
}", maskedJson);
        }

        [Fact]
        public void MaskLastSymbols()
        {
            var json =
@"{
   ""code"":  ""code"",
  ""test"": ""test"",
}";

            var maskedJson = _masker.Mask(json, p => p.Name == "code", MaskingStrategies.LastSymbolsByPercent(25));

            Assert.Equal(
@"{
   ""code"":  ""c***"",
  ""test"": ""test"",
}", maskedJson);
        }
    }
}
