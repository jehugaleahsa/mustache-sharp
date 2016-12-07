using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Mustache.Test
{
    [TestClass]
    public class JsonIfTests
    {
        private void AssertJsonReturnsExpected(string json, string expected)
        {
            var compiler = new HtmlFormatCompiler();
            const string format = @"Hello{{#if ifTrue}}, {{name}}{{/if}}!!!";

            var generator = compiler.Compile(format);
            dynamic jsonObject = JsonConvert.DeserializeObject<dynamic>(json);
            string actual = generator.Render(jsonObject);
            Assert.AreEqual(expected, actual, "The wrong message was generated.");
        }

        [TestMethod]
        public void NonEmptyArrayShows()
        {
            string json = "{'name': 'FirstName', ifTrue: [1]}";
            string expected = "Hello, FirstName!!!";
            AssertJsonReturnsExpected(json, expected);
        }

        [TestMethod]
        public void EmptyArrayHides()
        {
            string json = "{'name': 'FirstName', ifTrue: []}";
            string expected = "Hello!!!";
            AssertJsonReturnsExpected(json, expected);
        }

        [TestMethod]
        public void ZeroIntegerHides()
        {
            string json = "{'name': 'FirstName', ifTrue: 0}";
            string expected = "Hello!!!";
            AssertJsonReturnsExpected(json, expected);
        }

        [TestMethod]
        public void NonZeroIntegerShows()
        {
            string json = "{'name': 'FirstName', ifTrue: 1}";
            string expected = "Hello, FirstName!!!";
            AssertJsonReturnsExpected(json, expected);
        }

        [TestMethod]
        public void ZeroFloatHides()
        {
            string json = "{'name': 'FirstName', ifTrue: 0.0}";
            string expected = "Hello!!!";
            AssertJsonReturnsExpected(json, expected);
        }

        [TestMethod]
        public void NonZeroFloatShows()
        {
            string json = "{'name': 'FirstName', ifTrue: 99.0}";
            string expected = "Hello, FirstName!!!";
            AssertJsonReturnsExpected(json, expected);
        }

        [TestMethod]
        public void NullHides()
        {
            string json = "{'name': 'FirstName', ifTrue: null}";
            string expected = "Hello!!!";
            AssertJsonReturnsExpected(json, expected);
        }

        [TestMethod]
        public void FalseHides()
        {
            string json = "{'name': 'FirstName', ifTrue: false}";
            string expected = "Hello!!!";
            AssertJsonReturnsExpected(json, expected);
        }

        [TestMethod]
        public void TrueShows()
        {
            string json = "{'name': 'FirstName', ifTrue: true}";
            string expected = "Hello, FirstName!!!";
            AssertJsonReturnsExpected(json, expected);
        }
        [TestMethod]
        public void EmptyStringHides()
        {
            string json = "{'name': 'FirstName', ifTrue: ''}";
            string expected = "Hello!!!";
            AssertJsonReturnsExpected(json, expected);
        }

        [TestMethod]
        public void NonEmptyStringShows()
        {
            string json = "{'name': 'FirstName', ifTrue: 'asdf'}";
            string expected = "Hello, FirstName!!!";
            AssertJsonReturnsExpected(json, expected);
        }
    }
}