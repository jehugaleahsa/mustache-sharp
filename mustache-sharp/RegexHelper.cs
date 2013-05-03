using System;
using System.Text.RegularExpressions;

namespace Mustache
{
    /// <summary>
    /// Provides utility methods that require regular expressions.
    /// </summary>
    public static class RegexHelper
    {
        private const string Key = @"[_\w][_\w\d]*";
        internal const string CompoundKey = Key + @"(\." + Key + ")*";

        /// <summary>
        /// Determines whether the given name is a legal identifier.
        /// </summary>
        /// <param name="name">The name to check.</param>
        /// <returns>True if the name is a legal identifier; otherwise, false.</returns>
        public static bool IsValidIdentifier(string name)
        {
            if (name == null)
            {
                return false;
            }
            Regex regex = new Regex("^" + Key + "$");
            return regex.IsMatch(name);
        }
    }
}
