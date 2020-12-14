using Xunit;

namespace JsonMask.Tests
{
    public class TextCursorTest
    {
        [Fact]
        public void StartPosition()
        {
            var cursor = new TextCursor("sfsfsdsf\ndfsfdsf".ToCharArray());

            Assert.Equal(0, cursor.Position);
        }

        [Fact]
        public void GotoLine()
        {
            var cursor = new TextCursor("sfsfsdsf\ndfsfdsf".ToCharArray());

            cursor.GoTo(2, 2);

            Assert.Equal(11, cursor.Position);
        }

        [Fact]
        public void FindSymbol()
        {
            var cursor = new TextCursor("sfsfsdsf\ndfsfdsf".ToCharArray());
            cursor.GoTo(2, 0);

            cursor.FindSymbol('s');

            Assert.Equal(11, cursor.Position);
        }

        [Fact]
        public void NextGotoLine()
        {
            var cursor = new TextCursor("sfsfsdsf\ndfsfdsf".ToCharArray());
            cursor.GoTo(2, 2);

            cursor.GoTo(2, 4);

            Assert.Equal(13, cursor.Position);
        }
    }
}
