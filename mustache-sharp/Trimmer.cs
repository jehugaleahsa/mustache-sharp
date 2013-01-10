using System;

namespace mustache
{
    /// <summary>
    /// Removes unnecessary whitespace from static text.
    /// </summary>
    internal sealed class Trimmer
    {
        private bool hasHeader;
        private bool hasFooter;
        private bool hasTag;
        private bool canTrim;

        /// <summary>
        /// Initializes a new instance of a Trimmer.
        /// </summary>
        public Trimmer()
        {
            hasTag = false;
            canTrim = true;
        }

        /// <summary>
        /// Processes the given text, creating a StaticGenerator and adding it to the current compound generator.
        /// </summary>
        /// <param name="generator">The compound generator to add the static generator to.</param>
        /// <param name="isHeader">Gets whether we're encountered the header tag.</param>
        /// <param name="value">The static text to trim.</param>
        public void AddStaticGeneratorBeforeTag(CompoundGenerator generator, bool isHeader, string value)
        {
            string trimmed = processLines(value);
            hasHeader |= isHeader;
            hasFooter |= hasHeader && !isHeader;
            addStaticGenerator(generator, trimmed);
        }

        /// <summary>
        /// Processes the given text, creating a StaticGenerator and adding it to the current compound generator.
        /// </summary>
        /// <param name="generator">The compound generator to add the static generator to.</param>
        /// <param name="isOutput">Specifies whether the tag results in output.</param>
        /// <param name="value">The static text to trim.</param>
        public void AddStaticGenerator(CompoundGenerator generator, bool isOutput, string value)
        {
            string trimmed = processLines(value);
            canTrim &= !isOutput;
            addStaticGenerator(generator, trimmed);
        }

        private string processLines(string value)
        {
            string trimmed = value;
            int newline = value.IndexOf(Environment.NewLine);
            if (newline == -1)
            {
                canTrim &= String.IsNullOrWhiteSpace(value);
            }
            else
            {
                // finish processing the previous line
                if (canTrim && hasTag && (!hasHeader || !hasFooter))
                {
                    string lineEnd = trimmed.Substring(0, newline);
                    if (String.IsNullOrWhiteSpace(lineEnd))
                    {
                        trimmed = trimmed.Substring(newline + Environment.NewLine.Length);
                    }
                }
                // start processing the next line
                hasTag = false;
                hasHeader = false;
                hasFooter = false;
                int lastNewline = value.LastIndexOf(Environment.NewLine);
                string lineStart = value.Substring(lastNewline + Environment.NewLine.Length);
                canTrim = String.IsNullOrWhiteSpace(lineStart);
            }
            return trimmed;
        }

        private static void addStaticGenerator(CompoundGenerator generator, string trimmed)
        {
            if (trimmed.Length > 0)
            {
                StaticGenerator leading = new StaticGenerator(trimmed);
                generator.AddGenerator(leading);
            }
        }
    }
}