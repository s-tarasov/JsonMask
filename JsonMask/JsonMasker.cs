﻿using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text;

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

        protected virtual StringBuilder GetStringBuilder(string json)
            => new StringBuilder(json);

        public string MaskByPropertyName(string json, string propertyName)
            => Mask(json, p => p.Name == propertyName);

        public string MaskByPropertyName(string json, params string[] propertyNames)
            => Mask(json, p => propertyNames.Contains(p.Name));

        public string Mask(string json, FieldSelector selector, MaskValue maskValue = null)
        {
            if (string.IsNullOrEmpty(json))
                throw new ArgumentException("json must be not empty.", nameof(json));
            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            var chars = json.ToCharArray();

            using (var jsonTextReader = new JsonTextReader(new StringReader(json)))
                ReadAndMask(jsonTextReader, chars, selector, maskValue ?? MaskingStrategies.Full);

            var maskedJson = new string(chars);
            return maskedJson;
        }

        internal static void ReadAndMask(JsonTextReader reader, char[] chars,
            FieldSelector fieldSelector, MaskValue maskValue)
        {
            var walker = new CharsWalker(chars);
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
                    walker.GoTo(prevLine, prevLinePosition);
                    walker.FindSymbol('"');
                    var startPosition = walker.Position + 1;

                    walker.GoTo(reader.LineNumber, reader.LinePosition);
                    walker.FindSymbolBackWard('"');
                    var endPosition = walker.Position - 1;
                    maskValue(chars.AsSpan(startPosition, endPosition - startPosition + 1));
                }
                prevLine = reader.LineNumber;
                prevLinePosition = reader.LinePosition;
            }
            while (reader.Read());
        }
    }
    
}
