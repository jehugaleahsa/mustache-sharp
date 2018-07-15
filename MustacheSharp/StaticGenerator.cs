using System;
using System.IO;

namespace Mustache
{
    /// <summary>
    /// Generates a static block of text.
    /// </summary>
    internal sealed class StaticGenerator : IGenerator
    {
        /// <summary>
        /// Initializes a new instance of a StaticGenerator.
        /// </summary>
        public StaticGenerator(string value, bool removeNewLines)
        {
            if (removeNewLines)
            {
                Value = value.Replace(Environment.NewLine, String.Empty);
            }
            else
            {
                Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the static text.
        /// </summary>
        public string Value { get; }

        void IGenerator.GetText(TextWriter writer, Scope scope, Scope context, Action<Substitution> postProcessor)
        {
            writer.Write(Value);
        }
    }
}
