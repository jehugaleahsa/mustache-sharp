using System;
using System.IO;

namespace Mustache
{
    /// <summary>
    /// Substitutes a key placeholder with the textual representation of the
    /// associated object without escaping the output.
    /// </summary>
    internal sealed class LiteralKeyGenerator : IGenerator
    {
        private readonly string _key;
        private readonly bool _isVariable;

        /// <summary>
        /// Initializes a new instance of a LiteralKeyGenerator.
        /// </summary>
        /// <param name="key">The key to substitute with its value.</param>
        public LiteralKeyGenerator(string key)
        {
            if (key.StartsWith("@"))
            {
                _key = key.Substring(1);
                _isVariable = true;
            }
            else
            {
                _key = key;
                _isVariable = false;
            }
        }

        void IGenerator.GetText(Scope scope, TextWriter writer, Scope context)
        {
            object value = _isVariable ? context.Find(_key) : scope.Find(_key);
            writer.Write(value);
        }
    }
}
