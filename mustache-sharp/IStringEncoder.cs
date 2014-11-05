using System.IO;

namespace Mustache
{
    public interface IStringEncoder
    {
        void WriteEncoded(string input, TextWriter textWriter);
    }
}