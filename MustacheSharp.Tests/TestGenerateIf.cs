using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mustache.Test {
    [TestClass]
    public class TestGenerateIf {
        [TestMethod]
        public void TestGenerate_WithoutIfOnDateTime() {            
            FormatCompiler compiler = new FormatCompiler();
            const string format = @"{{DateOffCom:yyyy-MM-dd}}";
            Generator generator = compiler.Compile(format);
            string result = generator.Render(new { DateOffCom = new DateTime(2020,1,1) });

            Assert.AreEqual("2020-01-01", result.Substring(0, 10));
        }

        [TestMethod]
        public void TestGenerate_WithIfOnDateTime() {
            FormatCompiler compiler = new FormatCompiler();
            const string format = @"{{#if DateOffCom}}{{DateOffCom:yyyy-MM-dd}}{{/if}}";
            Generator generator = compiler.Compile(format);
            string result = generator.Render(new { DateOffCom = new DateTime(2020, 1, 1) });

            Assert.AreEqual("2020-01-01", result.Substring(0, 10));
        }

        [TestMethod]
        public void TestGenerate_WithIfOnDateTimeDefaultValue() {
            FormatCompiler compiler = new FormatCompiler();
            const string format = @"{{#if DateOffCom}}{{DateOffCom:yyyy-MM-dd}}{{/if}}";
            Generator generator = compiler.Compile(format);
            string result = generator.Render(new { DateOffCom = default(DateTime) });

            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void TestGenerate_WithIfOnDateTimeAndExtraString() {
            FormatCompiler compiler = new FormatCompiler();
            const string format = @"{{#if DateOffCom}}Date:{{DateOffCom:yyyy-MM-dd}}{{/if}}";
            Generator generator = compiler.Compile(format);
            string result = generator.Render(new { DateOffCom = new DateTime(2020, 1, 1) });

            Assert.AreEqual("Date:2020-01-01", result.Substring(0, 15));
        }
    }
}
