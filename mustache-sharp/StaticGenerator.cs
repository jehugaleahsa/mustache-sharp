using System;
using System.Collections.Generic;

namespace mustache
{
    /// <summary>
    /// Generates a static block of text.
    /// </summary>
    internal sealed class StaticGenerator : IGenerator
    {
        private readonly string _value;

        /// <summary>
        /// Initializes a new instance of a StaticGenerator.
        /// </summary>
        /// <param name="value">The string to return.</param>
        public StaticGenerator(string value)
        {
            _value = value;
        }

        string IGenerator.GetText(IFormatProvider provider, KeyScope scope)
        {
            return _value;
        }
    }
}
