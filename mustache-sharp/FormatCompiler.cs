using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using mustache.Properties;

namespace mustache
{
    /// <summary>
    /// Parses a format string and returns a text generator.
    /// </summary>
    public sealed class FormatCompiler
    {
        private const string key = @"[_\w][_\w\d]*";
        private const string compoundKey = key + @"(\." + key + ")*";

        private readonly MasterTagDefinition _master;
        private readonly TagScope _tagScope;

        /// <summary>
        /// Initializes a new instance of a FormatCompiler.
        /// </summary>
        public FormatCompiler()
        {
            _master = new MasterTagDefinition();
            _tagScope = new TagScope();
            registerTags(_master, _tagScope);
        }

        /// <summary>
        /// Registers the given tag definition with the parser.
        /// </summary>
        /// <param name="definition">The tag definition to register.</param>
        public void RegisterTag(TagDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException("definition");
            }
            _tagScope.AddTag(definition);
        }

        /// <summary>
        /// Builds a text generator based on the given format.
        /// </summary>
        /// <param name="format">The format to parse.</param>
        /// <returns>The text generator.</returns>
        public Generator Compile(string format)
        {
            CompoundGenerator generator = new CompoundGenerator(_master, new ArgumentCollection());
            int formatIndex = buildCompoundGenerator(_master, _tagScope, generator, format, 0);
            string trailing = format.Substring(formatIndex);
            StaticGenerator staticGenerator = new StaticGenerator(trailing);
            generator.AddGenerator(staticGenerator);
            return new Generator(generator);
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
            matches.Add(getCommentTagRegex());
            foreach (TagDefinition closingTag in definition.ClosingTags)
            {
                matches.Add(getClosingTagRegex(closingTag));
            }
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
            regexBuilder.Append(@"(?<close>(/(?<name>");
            regexBuilder.Append(definition.Name);
            regexBuilder.Append(@")\s*?))");
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

        private static int buildCompoundGenerator(
            TagDefinition tagDefinition, 
            TagScope scope, 
            CompoundGenerator generator, 
            string format, int formatIndex)
        {
            while (true)
            {
                Match match = findNextTag(tagDefinition, format, formatIndex);

                if (!match.Success)
                {
                    if (tagDefinition.ClosingTags.Any())
                    {
                        string message = String.Format(Resources.MissingClosingTag, tagDefinition.Name);
                        throw new FormatException(message);
                    }
                    break;
                }

                string leading = format.Substring(formatIndex, match.Index - formatIndex);
                StaticGenerator staticGenerator = new StaticGenerator(leading);
                generator.AddGenerator(staticGenerator);

                if (match.Groups["key"].Success)
                {
                    formatIndex = match.Index + match.Length;
                    string key = match.Groups["key"].Value;
                    string alignment = match.Groups["alignment"].Value;
                    string formatting = match.Groups["format"].Value;
                    KeyGenerator keyGenerator = new KeyGenerator(key, alignment, formatting);
                    generator.AddGenerator(keyGenerator);
                }
                else if (match.Groups["open"].Success)
                {
                    formatIndex = match.Index + match.Length;
                    string tagName = match.Groups["name"].Value;
                    TagDefinition nextDefinition = scope.Find(tagName);
                    if (nextDefinition == null)
                    {
                        string message = String.Format(Resources.UnknownTag, tagName);
                        throw new FormatException(message);
                    }
                    if (nextDefinition.HasBody)
                    {
                        ArgumentCollection arguments = getArguments(nextDefinition, match);
                        CompoundGenerator compoundGenerator = new CompoundGenerator(nextDefinition, arguments);
                        TagScope nextScope = new TagScope(scope);
                        registerTags(nextDefinition, nextScope);
                        formatIndex = buildCompoundGenerator(nextDefinition, nextScope, compoundGenerator, format, formatIndex);
                        generator.AddGenerator(nextDefinition, compoundGenerator);
                    }
                    else
                    {
                        Match nextMatch = findNextTag(nextDefinition, format, formatIndex);
                        ArgumentCollection arguments = getArguments(nextDefinition, nextMatch);
                        InlineGenerator inlineGenerator = new InlineGenerator(nextDefinition, arguments);
                        generator.AddGenerator(inlineGenerator);
                    }
                }
                else if (match.Groups["close"].Success)
                {
                    string tagName = match.Groups["name"].Value;
                    formatIndex = match.Index;
                    if (tagName == tagDefinition.Name)
                    {
                        formatIndex += match.Length;
                    }
                    break;
                }
                else if (match.Groups["comment"].Success)
                {
                    formatIndex = match.Index + match.Length;
                }
            }
            return formatIndex;
        }

        private static ArgumentCollection getArguments(TagDefinition definition, Match match)
        {
            ArgumentCollection collection = new ArgumentCollection();
            List<Capture> captures = match.Groups["argument"].Captures.Cast<Capture>().ToList();
            List<TagParameter> parameters = definition.Parameters.ToList();
            if (captures.Count > parameters.Count)
            {
                string message = String.Format(Resources.WrongNumberOfArguments, definition.Name);
                throw new FormatException(message);
            }
            if (captures.Count < parameters.Count)
            {
                captures.AddRange(Enumerable.Repeat((Capture)null, parameters.Count - captures.Count));
            }
            foreach (var pair in parameters.Zip(captures, (p, c) => new { Capture = c, Parameter = p }))
            {
                if (pair.Capture == null)
                {
                    if (pair.Parameter.IsRequired)
                    {
                        string message = String.Format(Resources.WrongNumberOfArguments, definition.Name);
                        throw new FormatException(message);
                    }
                    collection.AddArgument(pair.Parameter, null);
                }
                else
                {
                    collection.AddArgument(pair.Parameter, pair.Capture.Value);
                }                
            }
            return collection;
        }
    }
}
