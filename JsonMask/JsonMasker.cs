using Newtonsoft.Json;
using System;
using System.Buffers;
using System.IO;
using System.Linq;

namespace JsonMask
{
    public class JsonMasker
    {
        public struct PropertyInfo
        {
            private readonly JsonReader _reader;

            internal PropertyInfo(JsonReader reader, string name)
            {
                _reader = reader;
                Name = name;
            }

            public string Name { get; }

            public string GetPath() => _reader.Path;
        }

        public delegate bool FieldSelector(PropertyInfo filedIfo);

        public delegate void MaskValue(Span<char> value);

        public static ArrayPool<char> CharArrayPool { get; set; } = ArrayPool<char>.Shared;

        public string MaskByPropertyName(string json, string propertyName, MaskValue maskValue = null)
            => Mask(json, p => p.Name == propertyName, maskValue);

        public string MaskByPropertyName(string json, string[] propertyNames, MaskValue maskValue = null)
            => Mask(json, p => propertyNames.Contains(p.Name), maskValue);        

        public string Mask(string json, FieldSelector selector, MaskValue maskValue = null)
        {
            if (string.IsNullOrEmpty(json))
                throw new ArgumentException("json must be not empty.", nameof(json));
            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            maskValue = maskValue ?? MaskingStrategies.Full;

            var chars = CharArrayPool.Rent(json.Length);
            json.CopyTo(0, chars, 0, json.Length);

            using (var jsonTextReader = new JsonTextReader(new StringReader(json)))
                ReadAndMask(jsonTextReader, chars, selector, maskValue);

            var maskedJson = new string(chars, 0, json.Length);

            CharArrayPool.Return(chars);

            return maskedJson;
        }

        internal static void ReadAndMask(JsonTextReader reader, char[] chars, 
            FieldSelector fieldSelector, MaskValue maskValue)
        {
            var cursor = new TextCursor(chars);
            string propertyName = null;
            var prevLine = 0;
            var prevLinePosition = 0;
            do
            {
                if (reader.TokenType == JsonToken.PropertyName)
                    propertyName = reader.Value as string;

                if (reader.TokenType == JsonToken.String && propertyName != null
                    && fieldSelector(new PropertyInfo(reader, propertyName)))
                {
                    cursor.GoTo(prevLine, prevLinePosition);
                    cursor.FindSymbol('"');
                    var startPosition = cursor.Position + 1;

                    cursor.GoTo(reader.LineNumber, reader.LinePosition);
                    cursor.FindSymbolBackWard('"');
                    var endPosition = cursor.Position - 1;
                    maskValue(chars.AsSpan(startPosition, endPosition - startPosition + 1));
                }
                prevLine = reader.LineNumber;
                prevLinePosition = reader.LinePosition;
            }
            while (reader.Read());
        }
    }
    
}
