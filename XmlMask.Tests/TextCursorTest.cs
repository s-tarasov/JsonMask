using Xunit;

namespace XmlMask.Tests
{
    public class TextCursorTest
    {
        [Fact]
        public void StartPosition()
        {
            var cursor = new TextCursor("sfsfsdsf\ndfsfdsf".ToCharArray());

            Assert.Equal(0, cursor.Position);
            Assert.Equal(1, cursor.LineNumber);
            Assert.Equal(0, cursor.LinePosition);
        }

        [Fact]
        public void Goto()
        {
            var cursor = new TextCursor("sfsfsdsf\ndfsfdsf".ToCharArray());

            cursor.GoTo(2, 2);

            Assert.Equal(11, cursor.Position);
            Assert.Equal(2, cursor.LineNumber);
            Assert.Equal(2, cursor.LinePosition);
        }

        [Fact]
        public void GoToNotWhiteSpaceSymbol()
        {
            var cursor = new TextCursor("\r\n \t sfs".ToCharArray());

            cursor.GoToNotWhiteSpaceSymbol();

            Assert.Equal(5, cursor.Position);
            Assert.Equal(2, cursor.LineNumber);
            Assert.Equal(3, cursor.LinePosition);
        }

        [Fact]
        public void GoToLastLineSymbol()
        {
            var cursor = new TextCursor(" \r\n \t sfs".ToCharArray());

            cursor.GoToLastLineSymbol();

            Assert.Equal(1, cursor.Position);
            Assert.Equal(1, cursor.LineNumber);
            Assert.Equal(1, cursor.LinePosition);
        }

        [Fact]
        public void FindSymbol()
        {
            var cursor = new TextCursor("sfsfsdsf\ndfsfdXf".ToCharArray());            
            cursor.FindSymbol('X');
            Assert.Equal(14, cursor.Position);
            Assert.Equal(2, cursor.LineNumber);
            Assert.Equal(5, cursor.LinePosition);
        }

        [Fact]
        public void NextGotoLine()
        {
            var cursor = new TextCursor("sfsfsdsf\ndfsfdsf".ToCharArray());
            cursor.GoTo(2, 2);

            cursor.GoTo(2, 4);

            Assert.Equal(13, cursor.Position);
            Assert.Equal(2, cursor.LineNumber);
            Assert.Equal(4, cursor.LinePosition);
        }
    }
}
