using System.Text;
using Xunit;

namespace JsonMask.Tests
{
    public class CharsWalkerTest
    {
        [Fact]
        public void StartPosition()
        {
            var walker = new CharsWalker("sfsfsdsf\ndfsfdsf".ToCharArray());

            Assert.Equal(0, walker.Position);
        }

        [Fact]
        public void GotoLine()
        {
            var walker = new CharsWalker("sfsfsdsf\ndfsfdsf".ToCharArray());

            walker.GoTo(2, 2);

            Assert.Equal(11, walker.Position);
        }

        [Fact]
        public void FindSymbol()
        {
            var walker = new CharsWalker("sfsfsdsf\ndfsfdsf".ToCharArray());
            walker.GoTo(2, 0);

            walker.FindSymbol('s');

            Assert.Equal(11, walker.Position);
        }

        [Fact]
        public void NextGotoLine()
        {
            var walker = new CharsWalker("sfsfsdsf\ndfsfdsf".ToCharArray());
            walker.GoTo(2, 2);

            walker.GoTo(2, 4);

            Assert.Equal(13, walker.Position);
        }
    }
}
