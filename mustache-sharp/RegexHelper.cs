using System;
using System.Text.RegularExpressions;

namespace mustache
{
    /// <summary>
    /// Provides utility methods that require regular expressions.
    /// </summary>
    public static class RegexHelper
    {
        /// <summary>
        /// Determines whether the given name is a legal identifier.
        /// </summary>
        /// <param name="name">The name to check.</param>
        /// <returns>True if the name is a legal identifier; otherwise, false.</returns>
        public static bool IsValidIdentifier(string name)
        {
            Regex regex = new Regex(@"^[_\w][_\w\d]*$");
            return regex.IsMatch(name);
        }
    }
}
