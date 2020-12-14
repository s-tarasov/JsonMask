using System;
using System.Linq;
using Xunit;

namespace XmlMask.Tests
{
    public class XmlMaskerTests
    {
        private readonly XmlMasker _masker = new XmlMasker();

        [Fact]
        public void MaskByElementName()
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

            var maskedXml = _masker.MaskByElementName(xml, "Price");

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

        [Fact]
        public void MaskMultileValueByElementName()
        {
            var xml =
@"<soap:Envelope
xmlns:soap=""http://www.w3.org/2003/05/soap-envelope/""
soap:encodingStyle=""http://www.w3.org/2003/05/soap-encoding"">

  <soap:Body xmlns:m=""http://www.example.org/stock"">
      <m:GetStockPriceResponse>
        <m:Price>
  	      1234567
		  
		  	      1234
        </m:Price>
		     <m:Price2>1234567</m:Price2>
    </m:GetStockPriceResponse>
  </soap:Body>

</soap:Envelope>";

            var maskedXml = _masker.MaskByElementName(xml, "Price");

            Assert.Equal(
@"<soap:Envelope
xmlns:soap=""http://www.w3.org/2003/05/soap-envelope/""
soap:encodingStyle=""http://www.w3.org/2003/05/soap-encoding"">

  <soap:Body xmlns:m=""http://www.example.org/stock"">
      <m:GetStockPriceResponse>
        <m:Price>
  	      *******
		  
		  	      ****
        </m:Price>
		     <m:Price2>1234567</m:Price2>
    </m:GetStockPriceResponse>
  </soap:Body>

</soap:Envelope>", maskedXml);
        }

        [Fact]
        public void MaskByPath()
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

            var maskedXml = _masker.MaskByXPath(xml, "//*[local-name() = 'Price2']");

            Assert.Equal(
@"<soap:Envelope
xmlns:soap=""http://www.w3.org/2003/05/soap-envelope/""
soap:encodingStyle=""http://www.w3.org/2003/05/soap-encoding"">

  <soap:Body xmlns:m=""http://www.example.org/stock"">
      <m:GetStockPriceResponse>
        <m:Price>1234567</m:Price>
		     <m:Price2>*******</m:Price2>
    </m:GetStockPriceResponse>
  </soap:Body>

</soap:Envelope>", maskedXml);
        }

        public void MaskLastSymbols()
        {
            var xml =
@"<soap:Envelope
xmlns:soap=""http://www.w3.org/2003/05/soap-envelope/""
soap:encodingStyle=""http://www.w3.org/2003/05/soap-encoding"">

  <soap:Body xmlns:m=""http://www.example.org/stock"">
      <m:GetStockPriceResponse>
        <m:Price>12345678</m:Price>
		     <m:Price2>12345678</m:Price2>
    </m:GetStockPriceResponse>
  </soap:Body>

</soap:Envelope>";

            var maskedXml = _masker.MaskByElementName(xml, "Price2", MaskingStrategies.LastSymbolsByPercent(25));

            Assert.Equal(
@"<soap:Envelope
xmlns:soap=""http://www.w3.org/2003/05/soap-envelope/""
soap:encodingStyle=""http://www.w3.org/2003/05/soap-encoding"">

  <soap:Body xmlns:m=""http://www.example.org/stock"">
      <m:GetStockPriceResponse>
        <m:Price>12345678</m:Price>
		     <m:Price2>12******</m:Price2>
    </m:GetStockPriceResponse>
  </soap:Body>

</soap:Envelope>", maskedXml);
            
        }
    }
}
