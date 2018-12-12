using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mustache.Test
{
    [TestClass]
    public class HtmlFormatCompilerTester
    {
        [TestMethod]
        public void ShouldEscapeValueContainingHTMLCharacters()
        {
            HtmlFormatCompiler compiler = new HtmlFormatCompiler();
            var generator = compiler.Compile("<html><body>Hello, {{Name}}!!!</body></html>");
            string html = generator.Render(new
            {
                Name = "John \"The Man\" Standford"
            });
            Assert.AreEqual("<html><body>Hello, John &quot;The Man&quot; Standford!!!</body></html>", html);
        }

        [TestMethod]
        public void ShouldIgnoreHTMLCharactersInsideTripleCurlyBraces()
        {
            HtmlFormatCompiler compiler = new HtmlFormatCompiler();
            var generator = compiler.Compile("<html><body>Hello, {{{Name}}}!!!</body></html>");
            string html = generator.Render(new
            {
                Name = "John \"The Man\" Standford"
            });
            Assert.AreEqual("<html><body>Hello, John \"The Man\" Standford!!!</body></html>", html);
        }

        [TestMethod]
        public void ShouldNotTouchValueContainingSingleCurlyBraces() {
            HtmlFormatCompiler compiler = new HtmlFormatCompiler();
            var generator = compiler.Compile("<html><head><style>{{Style}}</style></head><body><b>Bold</b> statement!</body></html>");
            string html = generator.Render(new 
            {
                Style = "b { color: red; }"
            });
            Assert.AreEqual("<html><head><style>b { color: red; }</style></head><body><b>Bold</b> statement!</body></html>", html);
        }

        [TestMethod]
        public void ShouldNotTouchValueContainingSingleCurlyBracesInsideTripleCurlyBraces() {
            HtmlFormatCompiler compiler = new HtmlFormatCompiler();
            var generator = compiler.Compile("<html><head><style>{{{Style}}}</style></head><body><b>Bold</b> statement!</body></html>");
            string html = generator.Render(new 
            {
                Style = "b { color: red; }"
            });
            Assert.AreEqual("<html><head><style>b { color: red; }</style></head><body><b>Bold</b> statement!</body></html>", html);
        }

    }
}
