using System;
using System.Buffers;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.XPath;

namespace XmlMask
{
    public class XmlMasker
    {
        private static XmlReaderSettings _xmlReaderSettings = new XmlReaderSettings
        {
            IgnoreWhitespace = true,
            IgnoreProcessingInstructions = true,
            XmlResolver = null
        };

        public static ArrayPool<char> CharArrayPool { get; set; } = ArrayPool<char>.Shared;

        public delegate void MaskValue(Span<char> value);

        public string MaskByElementName(string xml, string[] elementNames, MaskValue maskValue = null)
        {
            if (elementNames is null)
                throw new ArgumentNullException(nameof(elementNames));

            return MaskByElementName(xml, n => elementNames.Contains(n), maskValue);
        }

        public string MaskByElementName(string xml, string elementName, MaskValue maskValue = null)
            => MaskByElementName(xml, n => n == elementName, maskValue);


        public string MaskByElementName(string xml, Predicate<string> elementNameSelector, MaskValue maskValue = null)
        {
            if (string.IsNullOrEmpty(xml))
                throw new ArgumentException("xml must be not empty.", nameof(xml));
            if (elementNameSelector is null)
                throw new ArgumentNullException(nameof(elementNameSelector));

            maskValue = maskValue ?? MaskingStrategies.Full;

            var chars = CharArrayPool.Rent(xml.Length);
            xml.CopyTo(0, chars, 0, xml.Length);

            var cursor = new TextCursor(chars);
            using (var xmlTextReader = XmlReader.Create(new StringReader(xml), _xmlReaderSettings))
                while (xmlTextReader.Read())
                    if (xmlTextReader.NodeType == XmlNodeType.Element && elementNameSelector(xmlTextReader.LocalName))
                        ReadAndMask((IXmlLineInfo)xmlTextReader, chars, maskValue, ref cursor);

            var maskedJson = new string(chars, 0, xml.Length);

            CharArrayPool.Return(chars);
            
            return maskedJson;
        }

       public string MaskByXPath(string xml, string xPathSelector, MaskValue maskValue = null)
        {
            if (string.IsNullOrEmpty(xml))
                throw new ArgumentException("xml must be not empty.", nameof(xml));
            if (string.IsNullOrEmpty(xPathSelector))
                throw new ArgumentNullException(nameof(xPathSelector));

            var chars = CharArrayPool.Rent(xml.Length);
            xml.CopyTo(0, chars, 0, xml.Length);

            var cursor = new TextCursor(chars);
            using (var xmlTextReader = XmlReader.Create(new StringReader(xml), _xmlReaderSettings)) 
            {
                var document = new XPathDocument(xmlTextReader);
                foreach (XPathNavigator node in document.CreateNavigator().Select(xPathSelector))
                    if (node.IsNode)
                        ReadAndMask((IXmlLineInfo)node, chars, maskValue ?? MaskingStrategies.Full, ref cursor);
            }

            var maskedJson = new string(chars, 0, xml.Length);

            CharArrayPool.Return(chars);

            return maskedJson;
        }

        internal static void ReadAndMask(IXmlLineInfo info, char[] chars, MaskValue maskValue, ref TextCursor cursor)
        {
            cursor.GoTo(info.LineNumber, info.LinePosition);
            cursor.FindSymbol('>');
            var cursorAtStartPosition = cursor;
            var startPosition = cursor.Position + 1;

            cursor.FindSymbol('<');
            var endPosition = cursor.Position - 1;
            if (cursor.LineNumber == info.LineNumber)
                maskValue(chars.AsSpan(startPosition, endPosition - startPosition + 1));
            else 
            {
                cursor = cursorAtStartPosition;
                cursor.GoToNextSymbol();
                while (cursor.Position <= endPosition) 
                {
                    cursor.GoToNotWhiteSpaceSymbol();
                    var valueStartPosition = cursor.Position;
                    if (valueStartPosition >= endPosition)
                        return;
                    cursor.GoToLastLineSymbol();
                    var valueEndPosition = cursor.Position-1;
                    maskValue(chars.AsSpan(valueStartPosition, valueEndPosition - valueStartPosition + 1));
                }
            }

            return;
        }
    }
    
}
