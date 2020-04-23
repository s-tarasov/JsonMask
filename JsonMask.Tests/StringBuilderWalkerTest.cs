using System.Text;
using Xunit;

namespace JsonMask.Tests
{
    public class StringBuilderWalkerTest
    {
        [Fact]
        public void StartPosition()
        {
            var walker = new StringBuilderWalker(new StringBuilder("sfsfsdsf\ndfsfdsf"));

            Assert.Equal(0, walker.Position);
        }

        [Fact]
        public void GotoLine()
        {
            var walker = new StringBuilderWalker(new StringBuilder("sfsfsdsf\ndfsfdsf"));

            walker.GoTo(2, 2);

            Assert.Equal(11, walker.Position);
        }

        [Fact]
        public void FindSymbol()
        {
            var walker = new StringBuilderWalker(new StringBuilder("sfsfsdsf\ndfsfdsf"));
            walker.GoTo(2, 0);

            walker.FindSymbol('s');

            Assert.Equal(11, walker.Position);
        }

        [Fact]
        public void NextGotoLine()
        {
            var walker = new StringBuilderWalker(new StringBuilder("sfsfsdsf\ndfsfdsf"));
            walker.GoTo(2, 2);

            walker.GoTo(2, 4);

            Assert.Equal(13, walker.Position);
        }
    }
}
