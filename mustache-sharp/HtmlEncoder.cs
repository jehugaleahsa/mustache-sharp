using System.IO;
using System.Web;

namespace Mustache
{
    public class HtmlEncoder : IStringEncoder
    {
        public void WriteEncoded(string input, TextWriter textWriter)
        {
            HttpUtility.HtmlEncode(input, textWriter);
        }
    }
}