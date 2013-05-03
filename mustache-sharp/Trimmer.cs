using System;
using System.Collections.Generic;
using System.Linq;

namespace Mustache
{
    /// <summary>
    /// Removes unnecessary lines from the final output.
    /// </summary>
    internal sealed class Trimmer
    {
        private readonly LinkedList<LineDetails> _lines;
        private LinkedListNode<LineDetails> _currentLine;

        /// <summary>
        /// Initializes a new instance of a Trimmer.
        /// </summary>
        public Trimmer()
        {
            _lines = new LinkedList<LineDetails>();
            _currentLine = _lines.AddLast(new LineDetails());
        }

        /// <summary>
        /// Updates the state of the trimmer, indicating that the given text was encountered before an inline tag.
        /// </summary>
        /// <param name="value">The text at the end of the format string.</param>
        /// <param name="generator">The generator created for the inline tag.</param>
        /// <returns>A static generator containing the passed text.</returns>
        public IEnumerable<StaticGenerator> RecordText(string value, bool isTag, bool isOutput)
        {
            int newLineIndex = value.IndexOf(Environment.NewLine);
            if (newLineIndex == -1)
            {
                StaticGenerator generator = new StaticGenerator() { Value = value };
                _currentLine.Value.Generators.Add(generator);
                _currentLine.Value.HasTag |= isTag;
                _currentLine.Value.HasOutput |= !String.IsNullOrWhiteSpace(value);
                yield return generator;
            }
            else
            {
                string[] lines = value.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                // get the trailing generator
                string trailing = lines[0];
                StaticGenerator trailingGenerator = new StaticGenerator() { Value = trailing };
                _currentLine.Value.Generators.Add(trailingGenerator);
                _currentLine.Value.HasOutput |= !String.IsNullOrWhiteSpace(trailing);
                yield return trailingGenerator;

                // get the middle generators
                for (int lineIndex = 1; lineIndex < lines.Length - 1; ++lineIndex)
                {
                    string middle = lines[lineIndex];
                    StaticGenerator middleGenerator = new StaticGenerator() { Value = middle };
                    LineDetails middleDetails = new LineDetails() { HasTag = false };
                    _currentLine = _lines.AddLast(middleDetails);
                    _currentLine.Value.Generators.Add(middleGenerator);
                    _currentLine.Value.HasOutput = true;
                    yield return middleGenerator;
                }

                // get the leading generator
                string leading = lines[lines.Length - 1];
                StaticGenerator leadingGenerator = new StaticGenerator() { Value = leading };
                LineDetails details = new LineDetails() { HasTag = isTag };
                _currentLine = _lines.AddLast(details);
                _currentLine.Value.Generators.Add(leadingGenerator);
                _currentLine.Value.HasOutput = !String.IsNullOrWhiteSpace(leading);
                yield return leadingGenerator;
            }
            if (isOutput)
            {
                _currentLine.Value.HasOutput = true;
            }
        }

        public void Trim()
        {
            removeBlankLines();
            separateLines();
            removeEmptyGenerators();
        }

        private void removeBlankLines()
        {
            LinkedListNode<LineDetails> current = _lines.First;
            while (current != null)
            {
                LineDetails details = current.Value;
                LinkedListNode<LineDetails> temp = current;
                current = current.Next;
                if (details.HasTag && !details.HasOutput)
                {
                    foreach (StaticGenerator generator in temp.Value.Generators)
                    {
                        generator.Prune();
                    }
                    temp.List.Remove(temp);
                }
            }
        }

        private void separateLines()
        {
            LinkedListNode<LineDetails> current = _lines.First;
            while (current != _lines.Last)
            {
                List<StaticGenerator> generators = current.Value.Generators;
                StaticGenerator lastGenerator = generators[generators.Count - 1];
                lastGenerator.Value += Environment.NewLine;
                current = current.Next;
            }
        }

        private void removeEmptyGenerators()
        {
            LinkedListNode<LineDetails> current = _lines.First;
            while (current != null)
            {
                foreach (StaticGenerator generator in current.Value.Generators)
                {
                    if (generator.Value.Length == 0)
                    {
                        generator.Prune();
                    }
                }
                current = current.Next;
            }
        }

        private sealed class LineDetails
        {
            public LineDetails()
            {
                Generators = new List<StaticGenerator>();
            }

            public bool HasTag { get; set; }

            public List<StaticGenerator> Generators { get; set; }

            public bool HasOutput { get; set; }
        }
    }
}
