# JsonMask
JsonMask is .NET string utility libraries which provides operations for clearing secure or personal data.

## Usage Example

```c#
        private readonly JsonMasker _jsonMasker = new JsonMasker();
        private readonly XmlMasker _xmlMasker = new XmlMasker();

        [Fact]
        public void MaskJsonByPropertName()
        {
            var json =
@"{
   ""code"":  ""code"",
  ""test"": ""test"",
}";

            var maskedJson = _jsonMasker.MaskByPropertyName(json, new []{ "code"});

            Assert.Equal(
@"{
   ""code"":  ""****"",
  ""test"": ""test"",
}", maskedJson);
        }
        
        [Fact]
        public void MaskXmlByElementName()
        {
            var xml =
@"<soap:Envelope
xmlns:soap=""http://www.w3.org/2003/05/soap-envelope/""
soap:encodingStyle=""http://www.w3.org/2003/05/soap-encoding"">
  <soap:Body xmlns:m=""http://www.example.org/stock"">
      <m:GetStockPriceResponse>
        <m:Price>1234567</m:Price>
		     <m:Price2>1234567</m:Price2>
    </m:GetStockPriceResponse>
  </soap:Body>
</soap:Envelope>";

            var maskedXml = _xmlMasker.MaskByElementName(xml, "Price");

            Assert.Equal(
@"<soap:Envelope
xmlns:soap=""http://www.w3.org/2003/05/soap-envelope/""
soap:encodingStyle=""http://www.w3.org/2003/05/soap-encoding"">
  <soap:Body xmlns:m=""http://www.example.org/stock"">
      <m:GetStockPriceResponse>
        <m:Price>*******</m:Price>
		     <m:Price2>1234567</m:Price2>
    </m:GetStockPriceResponse>
  </soap:Body>
</soap:Envelope>", maskedXml);
        }
```

## Benchmarks

https://github.com/s-tarasov/JsonMask/tree/master/BenchmarkResults
