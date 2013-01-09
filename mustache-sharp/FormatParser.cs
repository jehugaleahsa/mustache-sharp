using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace mustache
{
    /// <summary>
    /// Parses a format string and returns the text generator.
    /// </summary>
    internal sealed class FormatParser
    {
        private const string key = @"[_\w][_\w\d]*";
        private const string compoundKey = key + @"(\." + key + ")*";

        /// <summary>
        /// Initializes a new instance of a FormatParser.
        /// </summary>
        public FormatParser()
        {
        }

        /// <summary>
        /// Builds a text generator based on the given format.
        /// </summary>
        /// <param name="format">The format to parse.</param>
        /// <returns>The text generator.</returns>
        public IGenerator Build(string format)
        {
            TagDefinition definition = new TagDefinition("builtins");
            definition.HasBody = true;
            CompoundGenerator generator = new CompoundGenerator();
            TagScope tagScope = new TagScope();
            registerTags(definition, tagScope);
            Match match = findNextTag(definition, format, 0);
            buildCompoundGenerator(definition, tagScope, generator, format, 0, match);
            return generator;
        }

        private static void registerTags(TagDefinition definition, TagScope scope)
        {
            foreach (TagDefinition childTag in definition.ChildTags)
            {
                scope.AddTag(childTag);
            }
        }

        private static Match findNextTag(TagDefinition definition, string format, int formatIndex)
        {
            List<string> matches = new List<string>();
            matches.Add(getKeyRegex());
            matches.Add(getClosingTagRegex(definition));
            matches.Add(getCommentTagRegex());
            foreach (TagDefinition childTag in definition.ChildTags)
            {
                matches.Add(getTagRegex(childTag));
            }
            string match = "{{(" + String.Join("|", matches) + ")}}";
            Regex regex = new Regex(match);
            return regex.Match(format, formatIndex);
        }

        private static string getClosingTagRegex(TagDefinition definition)
        {
            StringBuilder regexBuilder = new StringBuilder();
            regexBuilder.Append(@"(?<close>(/");
            regexBuilder.Append(definition.Name);
            regexBuilder.Append(@"\s*?))");
            return regexBuilder.ToString();
        }

        private static string getCommentTagRegex()
        {
            return @"(?<comment>#!.*?)";
        }

        private static string getKeyRegex()
        {
            return @"((?<key>" + compoundKey + @")(,(?<alignment>(-)?[\d]+))?(:(?<format>.*?))?)";
        }

        private static string getTagRegex(TagDefinition definition)
        {
            StringBuilder regexBuilder = new StringBuilder();
            regexBuilder.Append(@"(?<open>(#(?<name>");
            regexBuilder.Append(definition.Name);
            regexBuilder.Append(@")");
            foreach (TagParameter parameter in definition.Parameters)
            {
                regexBuilder.Append(@"\s+?");
                regexBuilder.Append(@"(?<argument>");
                regexBuilder.Append(compoundKey);
                regexBuilder.Append(@")");
            }
            regexBuilder.Append(@"\s*?))");
            return regexBuilder.ToString();
        }

        private static int buildCompoundGenerator(TagDefinition tagDefinition, TagScope scope, CompoundGenerator generator, string format, int formatIndex, Match match)
        {
            bool done = false;
            while (!done)
            {
                string leading = format.Substring(formatIndex, match.Index - formatIndex);
                formatIndex = match.Index + match.Length;

                if (match.Groups["comment"].Success)
                {
                    // TODO - process comment
                }
                else if (match.Groups["close"].Success)
                {
                    // TODO - process closing tag
                    done = true;
                }
                else if (match.Groups["open"].Success)
                {
                    string tagName = match.Groups["name"].Value;
                    TagDefinition nextDefinition = scope.Find(tagName);
                    if (nextDefinition == null)
                    {
                        // TODO - handle missing tag definition
                    }
                    if (nextDefinition.HasBody)
                    {
                        CompoundGenerator nextGenerator = new CompoundGenerator();
                        TagScope nextScope = new TagScope(scope);
                        registerTags(nextDefinition, nextScope);
                        Match nextMatch = findNextTag(nextDefinition, format, formatIndex);
                        formatIndex = buildCompoundGenerator(nextDefinition, nextScope, nextGenerator, format, formatIndex, nextMatch);
                        // TODO - grab the generated text and parameters and pass it to the tag's processor
                        // TODO - a parameter can be a key or a default value
                    }
                    else
                    {
                        // TODO - grab all of the parameters and pass them to the tag's generator
                        // TODO - a parameter can be a key or a default value
                    }
                }
                else if (match.Groups["key"].Success)
                {
                    string alignment = match.Groups["alignment"].Value;
                    string formatting = match.Groups["format"].Value;
                    // TODO - create a key generator
                }
            }
            return formatIndex;
        }
    }
}
