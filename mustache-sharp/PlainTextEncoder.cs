using System.IO;

namespace Mustache
{
    public class PlainTextEncoder : IStringEncoder
    {
        public void WriteEncoded(string input, TextWriter textWriter)
        {
            textWriter.Write(input);
        }
    }
}