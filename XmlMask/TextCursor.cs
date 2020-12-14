using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace XmlMask
{
    [DebuggerDisplay("{Position} {LineNumber}:{LinePosition} {Preview}")]
    internal struct TextCursor
    {
        private readonly char[] _chars;
        private int _lineFirstSymbolPosition;

        public TextCursor(char[] chars)
        {
            _chars = chars;
            Position = 0;
            LineNumber = 1;
            _lineFirstSymbolPosition = 0;
        }

        public string Preview => new string(_chars, Position, _chars.Length-Position);
        
        public int Position { get; private set; }
        public int LineNumber { get; private set; }
        public int LinePosition => Position - _lineFirstSymbolPosition;
        

        internal void GoTo(int line, int linePosition)
        {
            if (LineNumber < line)
            {
                GoToLineDontChangeLinePosition(line);
                Position += linePosition;
            }
            else
            {
                Position += linePosition - LinePosition;
            }
        }

        internal void FindSymbol(char symbol)
        {
            if (_chars[Position] == symbol)
                return;
            while (true)
            {
                GoToNextSymbol();
                if (_chars[Position] == symbol)
                    return;
            }
        }

        internal void GoToNotWhiteSpaceSymbol()
        {
            while (true)
            {
                var symbol = _chars[Position];
                if (!char.IsWhiteSpace(symbol))
                    return;
                GoToNextSymbol();
            }
        }

        /// <summary>
        /// Go TO CARRIAGE RETURN Or NEXT LINE symbol
        /// </summary>
        internal void GoToLastLineSymbol()
        {
            while (true) 
            {
                var symbol = _chars[Position];
                if (symbol == '\r' || symbol == '\n')
                    return;
                Position++;
            }
        }

        internal void FindSymbolBackWard(char symbol)
        {
            if (_chars[Position] == symbol)
                return;
            while (true)
                if (_chars[--Position] == symbol)
                    return;
        }

        private void GoToLineDontChangeLinePosition(int line)
        {
            while (LineNumber < line)
                GoToNextSymbol();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void GoToNextSymbol()
        {
            var ch = _chars[Position];
            if (ch == '\r')
            {
                if (_chars[Position + 1] == '\n')
                    Position++;

                LineNumber++;
                Position++;
                _lineFirstSymbolPosition = Position; 
            }
            else if (ch == '\n')
            {
                LineNumber++;
                Position++;
                _lineFirstSymbolPosition = Position;
            }
            else
                Position++;
        }
    }
}
