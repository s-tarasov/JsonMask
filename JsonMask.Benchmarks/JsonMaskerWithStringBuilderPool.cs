using Microsoft.Extensions.ObjectPool;
using System.Text;

namespace JsonMask.Benchmarks
{
    public class JsonMaskerWithStringBuilderPool : JsonMasker
    {
        // StringBuilder chunk array max size.  
        private const int StringBuilderMaxChunkSize = 8000;

        ObjectPool<StringBuilder> _sbPool;

        public JsonMaskerWithStringBuilderPool()
        {
            var objectPoolProvider = new DefaultObjectPoolProvider();
            _sbPool = objectPoolProvider.CreateStringBuilderPool();
        }

        protected override StringBuilder GetStringBuilder(string json)
        {
            if (json.Length > StringBuilderMaxChunkSize)
                return new StringBuilder(json);

            var sb = _sbPool.Get();
            sb.Append(json);
            return sb;
        }

        protected override void ReturnStringBuilder(StringBuilder stringBuilder)
        {
            if (stringBuilder.Length > StringBuilderMaxChunkSize)
                return;
            _sbPool.Return(stringBuilder);
        }
    }
}
