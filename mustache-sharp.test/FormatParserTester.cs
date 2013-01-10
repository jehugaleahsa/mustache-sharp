using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace mustache.test
{
    /// <summary>
    /// Tests the FormatParser class.
    /// </summary>
    [TestClass]
    public class FormatParserTester
    {
        /// <summary>
        /// Replaces placeholds with the actual value.
        /// </summary>
        [TestMethod]
        public void TestBuild_Key_ReplacesWithValue()
        {
            FormatCompiler parser = new FormatCompiler();
            const string format = @"Hello, {{Name}}!!!";
            Generator generator = parser.Compile(format);
            string result = generator.Render(new { Name = "Bob" });
            Assert.AreEqual("Hello, Bob!!!", result, "The wrong text was generated.");
        }

        /// <summary>
        /// Removes comments from the output.
        /// </summary>
        [TestMethod]
        public void TestBuild_Comment_RemovesComment()
        {
            FormatCompiler parser = new FormatCompiler();
            const string format = "Before{{#! This is a comment }}After";
            Generator generator = parser.Compile(format);
            string result = generator.Render(new object());
            Assert.AreEqual("BeforeAfter", result, "The wrong text was generated.");
        }

        /// <summary>
        /// If the condition evaluates to false, the content of an if statement should not be printed.
        /// </summary>
        [TestMethod]
        public void TestBuild_If_EvaluatesToFalse_SkipsContent()
        {
            FormatCompiler parser = new FormatCompiler();
            const string format = "Before{{#if this}}Content{{/if}}After";
            Generator generator = parser.Compile(format);
            string result = generator.Render(false);
            Assert.AreEqual("BeforeAfter", result, "The wrong text was generated.");
        }

        /// <summary>
        /// If the condition evaluates to false, the content of an if statement should not be printed.
        /// </summary>
        [TestMethod]
        public void TestBuild_If_EvaluatesToTrue_PrintsContent()
        {
            FormatCompiler parser = new FormatCompiler();
            const string format = "Before{{#if this}}Content{{/if}}After";
            Generator generator = parser.Compile(format);
            string result = generator.Render(true);
            Assert.AreEqual("BeforeContentAfter", result, "The wrong text was generated.");
        }

        /// <summary>
        /// If the condition evaluates to false, the content of an else statement should be printed.
        /// </summary>
        [TestMethod]
        public void TestBuild_IfElse_EvaluatesToFalse_PrintsElse()
        {
            FormatCompiler parser = new FormatCompiler();
            const string format = "Before{{#if this}}Yay{{#else}}Nay{{/if}}After";
            Generator generator = parser.Compile(format);
            string result = generator.Render(false);
            Assert.AreEqual("BeforeNayAfter", result, "The wrong text was generated.");
        }

        /// <summary>
        /// If the condition evaluates to true, the content of an if statement should be printed.
        /// </summary>
        [TestMethod]
        public void TestBuild_IfElse_EvaluatesToTrue_PrintsIf()
        {
            FormatCompiler parser = new FormatCompiler();
            const string format = "Before{{#if this}}Yay{{#else}}Nay{{/if}}After";
            Generator generator = parser.Compile(format);
            string result = generator.Render(true);
            Assert.AreEqual("BeforeYayAfter", result, "The wrong text was generated.");
        }

        /// <summary>
        /// Second else blocks will be interpreted as just another piece of text.
        /// </summary>
        [TestMethod]
        public void TestBuild_IfElse_TwoElses_IncludesSecondElseInElse()
        {
            FormatCompiler parser = new FormatCompiler();
            const string format = "Before{{#if this}}Yay{{#else}}Nay{{#else}}Bad{{/if}}After";
            Generator generator = parser.Compile(format);
            string result = generator.Render(false);
            Assert.AreEqual("BeforeNay{{#else}}BadAfter", result, "The wrong text was generated.");
        }

        /// <summary>
        /// If the if statement evaluates to true, its block should be printed.
        /// </summary>
        [TestMethod]
        public void TestBuild_IfElifElse_IfTrue_PrintsIf()
        {
            FormatCompiler parser = new FormatCompiler();
            const string format = "Before{{#if First}}First{{#elif Second}}Second{{#else}}Third{{/if}}After";
            Generator generator = parser.Compile(format);
            string result = generator.Render(new { First = true, Second = true });
            Assert.AreEqual("BeforeFirstAfter", result, "The wrong text was generated.");
        }

        /// <summary>
        /// If the elif statement evaluates to true, its block should be printed.
        /// </summary>
        [TestMethod]
        public void TestBuild_IfElifElse_ElifTrue_PrintsIf()
        {
            FormatCompiler parser = new FormatCompiler();
            const string format = "Before{{#if First}}First{{#elif Second}}Second{{#else}}Third{{/if}}After";
            Generator generator = parser.Compile(format);
            string result = generator.Render(new { First = false, Second = true });
            Assert.AreEqual("BeforeSecondAfter", result, "The wrong text was generated.");
        }

        /// <summary>
        /// If the elif statement evaluates to false, the else block should be printed.
        /// </summary>
        [TestMethod]
        public void TestBuild_IfElifElse_ElifFalse_PrintsElse()
        {
            FormatCompiler parser = new FormatCompiler();
            const string format = "Before{{#if First}}First{{#elif Second}}Second{{#else}}Third{{/if}}After";
            Generator generator = parser.Compile(format);
            string result = generator.Render(new { First = false, Second = false });
            Assert.AreEqual("BeforeThirdAfter", result, "The wrong text was generated.");
        }

        /// <summary>
        /// If the elif statement evaluates to false and there is no else statement, nothing should be printed.
        /// </summary>
        [TestMethod]
        public void TestBuild_IfElif_ElifFalse_PrintsNothing()
        {
            FormatCompiler parser = new FormatCompiler();
            const string format = "Before{{#if First}}First{{#elif Second}}Second{{/if}}After";
            Generator generator = parser.Compile(format);
            string result = generator.Render(new { First = false, Second = false });
            Assert.AreEqual("BeforeAfter", result, "The wrong text was generated.");
        }

        /// <summary>
        /// If there are two elif statements and the first is false, the second elif block should be printed.
        /// </summary>
        [TestMethod]
        public void TestBuild_IfElifElif_ElifFalse_PrintsSecondElif()
        {
            FormatCompiler parser = new FormatCompiler();
            const string format = "Before{{#if First}}First{{#elif Second}}Second{{#elif Third}}Third{{/if}}After";
            Generator generator = parser.Compile(format);
            string result = generator.Render(new { First = false, Second = false, Third = true });
            Assert.AreEqual("BeforeThirdAfter", result, "The wrong text was generated.");
        }

        /// <summary>
        /// If we pass an empty collection to an each statement, the content should not be printed.
        /// </summary>
        [TestMethod]
        public void TestBuild_Each_EmptyCollection_SkipsContent()
        {
            FormatCompiler parser = new FormatCompiler();
            const string format = "Before{{#each this}}{{this}}{{/each}}After";
            Generator generator = parser.Compile(format);
            string result = generator.Render(new int[0]);
            Assert.AreEqual("BeforeAfter", result, "The wrong text was generated.");
        }

        /// <summary>
        /// If we pass a populated collection to an each statement, the content should be printed
        /// for each item in the collection, using that item as the new scope context.
        /// </summary>
        [TestMethod]
        public void TestBuild_Each_PopulatedCollection_PrintsContentForEach()
        {
            FormatCompiler parser = new FormatCompiler();
            const string format = "Before{{#each this}}{{this}}{{/each}}After";
            Generator generator = parser.Compile(format);
            string result = generator.Render(new int[] { 1, 2, 3 });
            Assert.AreEqual("Before123After", result, "The wrong text was generated.");
        }

        /// <summary>
        /// The object replacing the placeholder should be used as the context of a with statement.
        /// </summary>
        [TestMethod]
        public void TestBuild_With_AddsScope()
        {
            FormatCompiler parser = new FormatCompiler();
            const string format = "Before{{#with Nested}}{{this}}{{/with}}After";
            Generator generator = parser.Compile(format);
            string result = generator.Render(new { Nested = "Hello" });
            Assert.AreEqual("BeforeHelloAfter", result, "The wrong text was generated.");
        }
    }
}
