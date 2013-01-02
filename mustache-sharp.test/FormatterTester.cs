using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Globalization;

namespace mustache.test
{
    /// <summary>
    /// Tests the Formatter class.
    /// </summary>
    [TestClass]
    public class FormatterTester
    {
        #region Real World Example

        /// <summary>
        /// The Formatter class is especially useful when performing simple mail merge operations. 
        /// Like String.Format, Formatter will substitute placeholders for actual values. In the case
        /// of Formatter, placeholders are indicated by name, rather than index and are wrapped with
        /// double curly braces: {{name}}. The name within the curly brace can include any characters,
        /// including whitespace, except for two or more adjacent right curly braces (}}).
        /// </summary>
        [TestMethod]
        public void TestFormatter_ReplaceNamedPlaceholdersWithFormats()
        {
            const string format = "Hello {{name}}! It is {{date:MM-dd-yyyy}}. You make {{income:C}} an hour.";
            Formatter formatter = new Formatter(format);
            string result1 = formatter.Format(new Dictionary<string, object>() 
            { 
                { "name", "Bob" }, 
                { "date", new DateTime(2012, 03, 11) },
                { "income", 32.8 }
            });
            Assert.AreEqual("Hello Bob! It is 03-11-2012. You make $32.80 an hour.", result1);
        }

        /// <summary>
        /// If we want to work with objects, rather than raw dictionaries, we can wrap the objects with
        /// property dictionaries.
        /// </summary>
        [TestMethod]
        public void TestFormatter_UseObject()
        {
            var person = new
            {
                Name = "Bob",
                Date = new DateTime(2012, 03, 11),
                Income = 32.8
            };
            const string format = "Hello {{Name}}! It is {{Date:MM-dd-yyyy}}. You make {{Income:C}} an hour.";
            Formatter formatter = new Formatter(format);
            string result1 = formatter.Format(person);
            Assert.AreEqual("Hello Bob! It is 03-11-2012. You make $32.80 an hour.", result1);
        }

        /// <summary>
        /// We can the Formatter to print out a list of items following a format.
        /// </summary>
        [TestMethod]
        public void TestFormatter_PrintList()
        {
            List<int> values = new List<int>() { 0, 1, 2, 3, 4 };
            const string format = "{{#each this}}{{this}} {{/each}}";
            Formatter formatter = new Formatter(format);
            string result = formatter.Format(values);
            Assert.AreEqual("0 1 2 3 4 ", result);
        }

        /// <summary>
        /// We can include some text conditionally.
        /// </summary>
        [TestMethod]
        public void TestFormatter_ConditionallyIncludeText()
        {
            Random random = new Random();
            int value = random.Next();
            bool isEven = value % 2 == 0;
            var data = new
            {
                Value = value,
                IsEven = isEven,
            };
            const string format = "{{Value}} {{#if IsEven}}is even{{#else}}is odd{{/if}}.";
            Formatter formatter = new Formatter(format);
            string result = formatter.Format(data);
            string expected = String.Format("{0}", value) + (isEven ? " is even." : " is odd.");
            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Multiple cases can be handled using if/elif/else.
        /// </summary>
        [TestMethod]
        public void TestFormatter_HandleCases()
        {
            const string format = @"{{#if No}}No{{#elif Yes}}Yes{{#else}}Maybe{{/if}}";
            Formatter formatter = new Formatter(format);
            var data = new
            {
                Yes = true,
                No = false,
            };
            string result = formatter.Format(data);
            Assert.AreEqual("Yes", result);
        }

        /// <summary>
        /// We should be able to combine tags anyway we want.
        /// </summary>
        [TestMethod]
        public void TestFormatter_Compound()
        {
            const string format = @"{{#with Customer}}
Hello{{#if FirstName}} {{FirstName}}{{/if}}:
{{/with}}
{{#! We only want to print out purchases if they have some. }}
{{#if Purchases}} 

You recently purchased:
{{#each Purchases}}
    {{Name}}: {{Quantity}} x {{Price:C}}
{{/each}}
Your total was: {{Total:C}}
{{/if}}

We thought you might be interested in buying: {{PromotionProduct}}.

Thank you,
{{#with Agent}}
{{Name}}
{{/with}}";

            Formatter formatter = new Formatter(format);
            var data = new
            {
                Customer = new
                {
                    FirstName = "Bob",
                },
                Purchases = new object[]
                {
                    new 
                    {
                        Name = "Donkey",
                        Quantity = 8,
                        Price = 1.23m,
                    },
                    new
                    {
                        Name = "Hammer",
                        Quantity = 1,
                        Price = 8.32m,
                    },
                },
                Total = 18.16m,
                PromotionProduct = "Sneakers",
                Agent = new
                {
                    Name = "Tom",
                },
            };
            string result = formatter.Format(data);
            Assert.AreEqual(@"Hello Bob:

You recently purchased:
    Donkey: 8 x $1.23
    Hammer: 1 x $8.32
Your total was: $18.16

We thought you might be interested in buying: Sneakers.

Thank you,
Tom
", result);
        }

        #endregion

        #region Argument Checking

        /// <summary>
        /// An exception should be thrown if the format string is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCtor_NullFormat_ThrowsException()
        {
            string format = null;
            new Formatter(format);
        }

        /// <summary>
        /// If we try to replace a placeholder that we do not have a lookup key for,
        /// an exception should be thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void TestFormat_MissingKey_ThrowsException()
        {
            Formatter formatter = new Formatter("{{unknown}}");
            IDictionary<string, object> lookup = new Dictionary<string, object>();
            formatter.Format(lookup);
        }

        /// <summary>
        /// A format exception should be thrown if there is not a matching closing if tag.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void TestFormat_MissingClosingIfTag_ThrowsException()
        {
            new Formatter("{{#if Bob}}Hello");
        }

        /// <summary>
        /// A format exception should be thrown if the matching closing tag is wrong.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void TestFormat_WrongClosingIfTag_ThrowsException()
        {
            new Formatter("{{#with this}}{{#if Bob}}Hello{{/with}}{{/if}}");
        }

        #endregion

        /// <summary>
        /// If we specify a right alignment, the output should be aligned to the right.
        /// </summary>
        [TestMethod]
        public void TestFormatter_WithRightAlignment_AlignsToRight()
        {
            string format = "{{Name,10}}";
            var instance = new
            {
                Name = "Bob"
            };
            PropertyDictionary dictionary = new PropertyDictionary(instance);
            string result = Formatter.Format(format, dictionary);
            Assert.AreEqual("       Bob", result, "The text was not aligned.");
        }

        /// <summary>
        /// If we specify a left alignment, the output should be aligned to the left.
        /// </summary>
        [TestMethod]
        public void TestFormatter_WithLeftAlignment_AlignsToLeft()
        {
            string format = "{{Name,-10}}";
            var instance = new
            {
                Name = "Bob"
            };
            PropertyDictionary dictionary = new PropertyDictionary(instance);
            string result = Formatter.Format(null, format, dictionary);
            Assert.AreEqual("Bob       ", result, "The text was not aligned.");
        }

        /// <summary>
        /// If we try to format an empty string, an empty string should be returned.
        /// </summary>
        [TestMethod]
        public void TestFormatter_EmptyFormat_ReturnsEmpty()
        {
            Formatter formatter = new Formatter(String.Empty);
            Dictionary<string, object> lookup = new Dictionary<string, object>();
            string result = formatter.Format(lookup);
            Assert.AreEqual(String.Empty, result, "The result should have been empty.");
        }

        /// <summary>
        /// If our format string is just a placeholder, than just the replacement value should be returned.
        /// </summary>
        [TestMethod]
        public void TestFormatter_FormatIsSinglePlaceholder_ReturnsReplaced()
        {
            Formatter formatter = new Formatter("{{name}}");
            Dictionary<string, object> lookup = new Dictionary<string, object>()
            {
                { "name", "test" }
            };
            string result = formatter.Format(lookup);
            Assert.AreEqual("test", result, "The result was wrong.");
        }

        /// <summary>
        /// We should be able to put just about anything inside of a placeholder, but it will
        /// not be treated like a placeholder.
        /// </summary>
        [TestMethod]
        public void TestFormatter_PlaceholderContainsSpecialCharacters_ReturnsUnreplaced()
        {
            Formatter formatter = new Formatter("{{ \\_@#$%^ }1233 abc}}");
            Dictionary<string, object> lookup = new Dictionary<string, object>()
            {
                { " \\_@#$%^ }1233 abc", "test" }
            };
            string result = formatter.Format(lookup);
            Assert.AreEqual("{{ \\_@#$%^ }1233 abc}}", result, "The result was wrong.");
        }

        /// <summary>
        /// If a lookup value is null, it should be replaced with an empty string.
        /// </summary>
        [TestMethod]
        public void TestFormatter_NullValue_ReplacesWithBlank()
        {
            Formatter formatter = new Formatter("These quotes should be empty '{{name}}'.");
            Dictionary<string, object> lookup = new Dictionary<string, object>()
            {
                { "name", null }
            };
            string result = formatter.Format(lookup);
            Assert.AreEqual("These quotes should be empty ''.", result, "The result was wrong.");
        }

        /// <summary>
        /// If a replacement value contains a placeholder, it should NOT be evaluated.
        /// </summary>
        [TestMethod]
        public void TestFormatter_ReplacementContainsPlaceholder_IgnoresPlaceholder()
        {
            Formatter formatter = new Formatter("The length of {{name}} is {{length}}.");
            Dictionary<string, object> lookup = new Dictionary<string, object>()
            {
                { "name", "Bob" },
                { "length", "{{name}}" }
            };
            string result = formatter.Format(lookup);
            Assert.AreEqual("The length of Bob is {{name}}.", result, "The result was wrong.");
        }

        /// <summary>
        /// If we pass null to as the format provider to the Format function,
        /// the current culture is used.
        /// </summary>
        [TestMethod]
        public void TestFormatter_NullFormatter_UsesCurrentCulture()
        {
            string format = "{0:C}";
            Formatter formatter = new Formatter("{" + format + "}");
            string result = formatter.Format((IFormatProvider)null, new Dictionary<string, object>() { { "0", 28.30m } });
            string expected = String.Format(CultureInfo.CurrentCulture, format, 28.30m);
            Assert.AreEqual(expected, result, "The wrong format provider was used.");
        }

        /// <summary>
        /// If we put a tag on a line by itself, it shouldn't result in any whitespace.
        /// </summary>
        [TestMethod]
        public void TestFormatter_TagOnLineByItself_NoNewlineGenerated()
        {
            const string format = @"Hello
{{#if Name}}
{{Name}}
{{/if}}
Goodbye
";
            var data = new { Name = "George" };
            Formatter formatter = new Formatter(format);
            string result = formatter.Format(data);
            const string expected = @"Hello
George
Goodbye
";
            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// If a key is not found at the current level, it is looked for at the parent level.
        /// </summary>
        [TestMethod]
        public void TestFormatter_NameAtHigherScope_Finds()
        {
            const string format = "{{#with Child}}{{TopLevel}} and {{ChildLevel}}{{/with}}";
            Formatter formatter = new Formatter(format);
            var data = new
            {
                TopLevel = "Parent",
                Child = new { ChildLevel = "Child" },
            };
            string result = formatter.Format(data);
            Assert.AreEqual("Parent and Child", result);
        }

        /// <summary>
        /// Null values are considered false by if statements.
        /// </summary>
        [TestMethod]
        public void TestFormatter_ConditionOnNull_ConsideredFalse()
        {
            const string format = "{{#if this}}Bad{{#else}}Good{{/if}}";
            Formatter formatter = new Formatter(format);
            string result = formatter.Format(null);
            Assert.AreEqual("Good", result);
        }

        /// <summary>
        /// Empty collections are considered false by if statements.
        /// </summary>
        [TestMethod]
        public void TestFormatter_ConditionOnEmptyCollection_ConsideredFalse()
        {
            const string format = "{{#if this}}Bad{{#else}}Good{{/if}}";
            Formatter formatter = new Formatter(format);
            string result = formatter.Format(new object[0]);
            Assert.AreEqual("Good", result);
        }

        /// <summary>
        /// Non-empty collections are considered true by if statements.
        /// </summary>
        [TestMethod]
        public void TestFormatter_ConditionOnNonEmptyCollection_ConsideredTrue()
        {
            const string format = "{{#if this}}Good{{#else}}Bad{{/if}}";
            Formatter formatter = new Formatter(format);
            string result = formatter.Format(new object[1]);
            Assert.AreEqual("Good", result);
        }

        /// <summary>
        /// Null-char is considered false by if statements.
        /// </summary>
        [TestMethod]
        public void TestFormatter_ConditionOnNullChar_ConsideredFalse()
        {
            const string format = "{{#if this}}Bad{{#else}}Good{{/if}}";
            Formatter formatter = new Formatter(format);
            string result = formatter.Format('\0');
            Assert.AreEqual("Good", result);
        }

        /// <summary>
        /// Zero is considered false by if statements.
        /// </summary>
        [TestMethod]
        public void TestFormatter_ConditionOnZero_ConsideredFalse()
        {
            const string format = "{{#if this}}Bad{{#else}}Good{{/if}}";
            Formatter formatter = new Formatter(format);
            int? value = 0;
            string result = formatter.Format(value);
            Assert.AreEqual("Good", result);
        }

        /// <summary>
        /// Everything else is considered true by if statements.
        /// </summary>
        [TestMethod]
        public void TestFormatter_ConditionOnDateTime_ConsideredTrue()
        {
            const string format = "{{#if this}}Good{{#else}}Bad{{/if}}";
            Formatter formatter = new Formatter(format);
            string result = formatter.Format(DateTime.Now);
            Assert.AreEqual("Good", result);
        }

        /// <summary>
        /// Instead of requiring deeply nested "with" statements, members
        /// can be separated by dots.
        /// </summary>
        [TestMethod]
        public void TestFormatter_NestedMembers_SearchesMembers()
        {
            const string format = "{{Customer.Name}}";
            Formatter formatter = new Formatter(format);
            var data = new { Customer = new { Name = "Bob" } };
            string result = formatter.Format(data);
            Assert.AreEqual("Bob", result);
        }

        /// <summary>
        /// Keys should cause newlines to be respected, since they are considered content.
        /// </summary>
        [TestMethod]
        public void TestFormatter_KeyBetweenTags_RespectsTrailingNewline()
        {
            string format = "{{#each this}}{{this}} {{/each}}" + Environment.NewLine;
            Formatter formatter = new Formatter(format);
            string result = formatter.Format("Hello");
            Assert.AreEqual("H e l l o " + Environment.NewLine, result);
        }

        /// <summary>
        /// If someone tries to loop on a non-enumerable, it should do nothing.
        /// </summary>
        [TestMethod]
        public void TestFormatter_EachOnNonEnumerable_PrintsNothing()
        {
            const string format = "{{#each this}}Bad{{/each}}";
            Formatter formatter = new Formatter(format);
            string result = formatter.Format(123);
            Assert.AreEqual(String.Empty, result);
        }

        /// <summary>
        /// If a tag header is on the same line as it's footer, the new-line should not be removed.
        /// </summary>
        [TestMethod]
        public void TestFormatter_InlineTags_RespectNewLine()
        {
            const string format = @"{{#if this}}{{/if}}
";
            Formatter formatter = new Formatter(format);
            string result = formatter.Format(true);
            Assert.AreEqual(Environment.NewLine, result);
        }

        /// <summary>
        /// If a tag header is on the same line as it's footer, the new-line should not be removed.
        /// </summary>
        [TestMethod]
        public void TestFormatter_TagFooterFollowedByTagHeader_RemovesNewLine()
        {
            const string format = @"{{#if this}}
{{/if}}{{#if this}}
Hello{{/if}}";
            Formatter formatter = new Formatter(format);
            string result = formatter.Format(true);
            Assert.AreEqual("Hello", result);
        }
    }
}
