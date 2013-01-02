using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using mustache.Properties;

namespace mustache
{
    /// <summary>
    /// Allows for the generation of a string based on formatted template.
    /// </summary>
    public sealed class Formatter
    {
        private readonly CompoundBuilder builder;

        /// <summary>
        /// Initializes a new instance of a Formatter using the given format string.
        /// </summary>
        /// <param name="format">The string containing the placeholders to use as a template.</param>
        /// <exception cref="System.ArgumentNullException">The format string is null.</exception>
        /// <exception cref="System.FormatException">The format string is invald.</exception>
        public Formatter(string format)
        {
            if (format == null)
            {
                throw new ArgumentNullException("format");
            }
            builder = new CompoundBuilder();

            List<string> names = new List<string>();
            const string key = @"[_\w][_\w\d]*";
            const string compoundKey = key + @"(\." + key + ")*";
            const string openIfMatch = @"(?<open_if>(#if\s+?" + compoundKey + @"\s*?))";
            const string elifMatch = @"(?<elif>(#elif\s+?" + compoundKey + @"\s*?))";
            const string elseMatch = @"(?<else>(#else\s*?))";
            const string closeIfMatch = @"(?<close_if>(/if\s*?))";
            const string openEachMatch = @"(?<open_each>(#each\s+?" + compoundKey + @"\s*?))";
            const string closeEachMatch = @"(?<close_each>(/each\s*?))";
            const string openWithMatch = @"(?<open_with>(#with\s+?" + compoundKey + @"\s*?))";
            const string closeWithMatch = @"(?<close_with>(/with\s*?))";
            const string commentMatch = @"(?<comment>#!.*?)";
            const string keyMatch = @"((?<key>" + compoundKey + @")(,(?<alignment>(-)?[\d]+))?(:(?<format>.*?))?)";
            const string match = "{{(" + openIfMatch + "|" 
                                      + elifMatch + "|" 
                                      + elseMatch + "|" 
                                      + closeIfMatch + "|" 
                                      + openEachMatch + "|"
                                      + closeEachMatch + "|"
                                      + openWithMatch + "|"
                                      + closeWithMatch + "|"
                                      + commentMatch + "|"
                                      + keyMatch + ")}}";
            Regex formatFinder = new Regex(match, RegexOptions.Compiled);
            List<Match> matches = formatFinder.Matches(format).Cast<Match>().ToList();
            using (IEnumerator<Match> matchEnumerator = matches.GetEnumerator())
            {
                Trimmer trimmer = new Trimmer();
                int formatIndex = buildCompoundBuilder(builder, trimmer, format, 0, matchEnumerator);
                StaticBuilder trailingBuilder = new StaticBuilder();
                string value = format.Substring(formatIndex);
                TagAttributes attributes = new TagAttributes() { Type = TagType.None, IsOutput = false };
                trimmer.AddStaticBuilder(builder, attributes, value);
            }
        }

        /// <summary>
        /// Substitutes the placeholders in the format string with the values found in the object.
        /// </summary>
        /// <param name="format">The string containing the placeholders to use as a template.</param>
        /// <param name="value">The object to use to replace the placeholders.</param>
        /// <returns>The format string with the placeholders substituted for by the object values.</returns>
        /// <exception cref="System.ArgumentNullException">The format string is null.</exception>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">A property was not found in the value.</exception>
        public static string Format(string format, object value)
        {
            Formatter formatter = new Formatter(format);
            return formatter.Format(value);
        }

        /// <summary>
        /// Substitutes the placeholders in the format string with the values found in the object.
        /// </summary>
        /// <param name="provider">The format provider to use -or- null to use the current culture.</param>
        /// <param name="format">The string containing the placeholders to use as a template.</param>
        /// <param name="value">The object to use to replace the placeholders.</param>
        /// <returns>The format string with the placeholders substituted for by the object values.</returns>
        /// <exception cref="System.ArgumentNullException">The format string is null.</exception>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">A property was not found in the value.</exception>
        public static string Format(IFormatProvider provider, string format, object value)
        {
            Formatter formatter = new Formatter(format);
            return formatter.Format(provider, value);
        }

        /// <summary>
        /// Substitutes the placeholders in the format string with the values found in the given object.
        /// </summary>
        /// <param name="value">The object to use to replace the placeholders.</param>
        /// <returns>The format string with the placeholders substituted for by the lookup values.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">A property was not found in the object.</exception>
        /// <remarks>A null value will be replaced with an empty string.</remarks>
        public string Format(object value)
        {
            return format(CultureInfo.CurrentCulture, value);
        }

        /// <summary>
        /// Substitutes the placeholders in the format string with the values found in the given object.
        /// </summary>
        /// <param name="provider">The format provider to use -or- null to use the current culture.</param>
        /// <param name="value">The object to use to replace the placeholders.</param>
        /// <returns>The format string with the placeholders substituted for by the lookup values.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">A property was not found in the object.</exception>
        /// <remarks>A null value will be replaced with an empty string.</remarks>
        public string Format(IFormatProvider provider, object value)
        {
            if (provider == null)
            {
                provider = CultureInfo.CurrentCulture;
            }
            return format(provider, value);
        }

        private static int buildCompoundBuilder(CompoundBuilder builder, Trimmer trimmer, string format, int formatIndex, IEnumerator<Match> matches)
        {
            while (matches.MoveNext())
            {
                Match match = matches.Current;
                string value = format.Substring(formatIndex, match.Index - formatIndex);
                formatIndex = match.Index + match.Length;

                Group keyGroup = match.Groups["key"];
                if (keyGroup.Success)
                {
                    TagAttributes attributes = new TagAttributes() { Type = TagType.Singleton, IsOutput = true };
                    trimmer.AddStaticBuilder(builder, attributes, value);
                    Group alignmentGroup = match.Groups["alignment"];
                    Group formatGroup = match.Groups["format"];
                    KeyBuilder keyBuilder = new KeyBuilder()
                    {
                        Key = keyGroup.Value,
                        Alignment = alignmentGroup.Value,
                        Format = formatGroup.Value,
                    };
                    builder.AddBuilder(keyBuilder);
                    continue;
                }

                Group openIfGroup = match.Groups["open_if"];
                if (openIfGroup.Success)
                {
                    TagAttributes attributes = new TagAttributes() { Type = TagType.Header, IsOutput = false };
                    trimmer.AddStaticBuilder(builder, attributes, value);
                    IfBuilder ifBuilder = new IfBuilder();
                    ifBuilder.Key = openIfGroup.Value.Substring(4).Trim();
                    formatIndex = buildIfBuilder(ifBuilder, true, trimmer, format, formatIndex, matches);
                    builder.AddBuilder(ifBuilder);
                    continue;
                }

                Group openEachGroup = match.Groups["open_each"];
                if (openEachGroup.Success)
                {
                    TagAttributes attributes = new TagAttributes() { Type = TagType.Header, IsOutput = false };
                    trimmer.AddStaticBuilder(builder, attributes, value);
                    EachBuilder eachBuilder = new EachBuilder();
                    eachBuilder.Key = openEachGroup.Value.Substring(6).Trim();
                    formatIndex = buildEachBuilder(eachBuilder, trimmer, format, formatIndex, matches);
                    builder.AddBuilder(eachBuilder);
                    continue;
                }

                Group openWithGroup = match.Groups["open_with"];
                if (openWithGroup.Success)
                {
                    TagAttributes attributes = new TagAttributes() { Type = TagType.Header, IsOutput = false };
                    trimmer.AddStaticBuilder(builder, attributes, value);
                    WithBuilder withBuilder = new WithBuilder();
                    withBuilder.Key = openWithGroup.Value.Substring(6).Trim();
                    formatIndex = buildWithBuilder(withBuilder, trimmer, format, formatIndex, matches);
                    builder.AddBuilder(withBuilder);
                    continue;
                }

                Group commentGroup = match.Groups["comment"];
                if (commentGroup.Success)
                {
                    TagAttributes attributes = new TagAttributes() { Type = TagType.Singleton, IsOutput = false };
                    trimmer.AddStaticBuilder(builder, attributes, value);
                    continue;
                }

                Group elifGroup = match.Groups["elif"];
                if (elifGroup.Success)
                {
                    TagAttributes attributes = new TagAttributes() { Type = TagType.Singleton, IsOutput = false };
                    trimmer.AddStaticBuilder(builder, attributes, value);
                    break;
                }

                Group elseGroup = match.Groups["else"];
                if (elseGroup.Success)
                {
                    TagAttributes attributes = new TagAttributes() { Type = TagType.Singleton, IsOutput = false };
                    trimmer.AddStaticBuilder(builder, attributes, value);
                    break;
                }

                Group closeIfGroup = match.Groups["close_if"];
                if (closeIfGroup.Success)
                {
                    TagAttributes attributes = new TagAttributes() { Type = TagType.Footer, IsOutput = false };
                    trimmer.AddStaticBuilder(builder, attributes, value);
                    break;
                }

                Group closeEachGroup = match.Groups["close_each"];
                if (closeEachGroup.Success)
                {
                    TagAttributes attributes = new TagAttributes() { Type = TagType.Footer, IsOutput = false };
                    trimmer.AddStaticBuilder(builder, attributes, value);
                    break;
                }

                Group closeWithGroup = match.Groups["close_with"];
                if (closeWithGroup.Success)
                {
                    TagAttributes attributes = new TagAttributes() { Type = TagType.Footer, IsOutput = false };
                    trimmer.AddStaticBuilder(builder, attributes, value);
                    break;
                }
            }
            return formatIndex;
        }

        private static int buildIfBuilder(IfBuilder builder, bool expectClosingTag, Trimmer trimmer, string format, int formatIndex, IEnumerator<Match> matches)
        {
            formatIndex = buildCompoundBuilder(builder.TrueBuilder, trimmer, format, formatIndex, matches);
            Match match = matches.Current;
            if (match != null)
            {
                Group elifGroup = match.Groups["elif"];
                if (elifGroup.Success)
                {
                    IfBuilder elifBuilder = new IfBuilder();
                    elifBuilder.Key = elifGroup.Value.Substring(6).Trim();
                    formatIndex = buildIfBuilder(elifBuilder, false, trimmer, format, formatIndex, matches);
                    builder.FalseBuilder.AddBuilder(elifBuilder);
                }
                else
                {
                    Group elseGroup = match.Groups["else"];
                    if (elseGroup.Success)
                    {
                        formatIndex = buildCompoundBuilder(builder.FalseBuilder, trimmer, format, formatIndex, matches);
                    }
                }
            }
            if (expectClosingTag)
            {
                Match closingMatch = matches.Current;
                checkClosingTag(closingMatch, "close_if", "if");
            }
            return formatIndex;
        }

        private static int buildEachBuilder(EachBuilder builder, Trimmer trimmer, string format, int formatIndex, IEnumerator<Match> matches)
        {
            formatIndex = buildCompoundBuilder(builder.Builder, trimmer, format, formatIndex, matches);
            Match closingMatch = matches.Current;
            checkClosingTag(closingMatch, "close_each", "each");
            return formatIndex;
        }

        private static int buildWithBuilder(WithBuilder builder, Trimmer trimmer, string format, int formatIndex, IEnumerator<Match> matches)
        {
            formatIndex = buildCompoundBuilder(builder.Builder, trimmer, format, formatIndex, matches);
            Match closingMatch = matches.Current;
            checkClosingTag(closingMatch, "close_with", "with");
            return formatIndex;
        }

        private static void checkClosingTag(Match match, string expectedTag, string openingTag)
        {
            if (match == null || !match.Groups[expectedTag].Success)
            {
                string errorMessage = String.Format(CultureInfo.CurrentCulture, Resources.MissingClosingTag, openingTag);
                throw new FormatException(errorMessage);
            }
        }

        private string format(IFormatProvider provider, object topLevel)
        {
            Scope scope = new Scope(topLevel);
            StringBuilder output = new StringBuilder();
            builder.Build(scope, output, provider);
            return output.ToString();
        }
    }
}
