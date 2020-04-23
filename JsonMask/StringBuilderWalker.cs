using System.Text;

namespace JsonMask
{
    internal struct StringBuilderWalker
    {
        private readonly StringBuilder _stringBuilder;
        private int _curLine;
        private int _curLinePosition;

        public StringBuilderWalker(StringBuilder stringBuilder)
        {
            _stringBuilder = stringBuilder;
            _curLine = 1;
            _curLinePosition = 0;
            Position = 0;
        }

        public int Position { get; internal set; }

        internal void GoTo(int line, int linePosition)
        {
            if (_curLine < line)
            {
                GoToLine(line);
                _curLinePosition = linePosition;
                Position += linePosition;
            }
            else
            {
                Position += linePosition - _curLinePosition;
                _curLinePosition = linePosition;
            }
        }

        internal void FindSymbol(char symbol)
        {
            if (_stringBuilder[Position] == symbol)
                return;
            while (true)
                if (_stringBuilder[++Position] == symbol)
                    return;
        }

        internal void FindSymbolBackWard(char symbol)
        {
            if (_stringBuilder[Position] == symbol)
                return;
            while (true)
                if (_stringBuilder[--Position] == symbol)
                    return;
        }

        private void GoToLine(int line)
        {
            while (_curLine < line)
            {
                var ch = _stringBuilder[Position];
                if (ch == '\r' || ch == '\n')
                {
                    if (ch == '\r' && _stringBuilder[Position + 1] == '\n')
                        Position++;
                    _curLine++;
                }
                Position++;
            }
        }
    }
}
