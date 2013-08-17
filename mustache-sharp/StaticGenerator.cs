using System;
using System.Collections.Generic;
using System.IO;

namespace Mustache
{
    /// <summary>
    /// Generates a static block of text.
    /// </summary>
    internal sealed class StaticGenerator : IGenerator
    {
        private readonly string value;

        /// <summary>
        /// Initializes a new instance of a StaticGenerator.
        /// </summary>
        public StaticGenerator(string value)
        {
            this.value = value.Replace(Environment.NewLine, String.Empty);
        }

        /// <summary>
        /// Gets or sets the static text.
        /// </summary>
        public string Value
        {
            get { return value; }
        }

        void IGenerator.GetText(Scope scope, TextWriter writer, Scope context)
        {
            writer.Write(Value);
        }
    }
}
