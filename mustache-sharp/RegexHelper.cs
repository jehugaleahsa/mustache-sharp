using System;
using System.Text.RegularExpressions;

namespace Mustache
{
    /// <summary>
    /// Provides utility methods that require regular expressions.
    /// </summary>
    internal static class RegexHelper
    {
        public const string Key = @"[_\w][_\w\d]*";
        public const string String = @"'.*?'";
        public const string Number = @"[-+]?\d*\.?\d+";
        public const string CompoundKey = "@?" + Key + @"(?:\." + Key + ")*";
        public const string Argument = @"(?:(?<arg_key>" + CompoundKeyOrArrayAccess + @")|(?<arg_string>" + String + @")|(?<arg_number>" + Number + @"))";
		public const string KeyOrArrayAccess = "(?:" + Key + @"|\[[^\]]+\]" + ")";
		public const string CompoundKeyOrArrayAccess = KeyOrArrayAccess + @"(\." + KeyOrArrayAccess + ")*";

		public static bool IsInteger(string s) {
			return Regex.IsMatch(s, @"^\d{1,10}$");
		}

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

        public static bool IsString(string value)
        {
            if (value == null)
            {
                return false;
            }
            Regex regex = new Regex("^" + String + "$");
            return regex.IsMatch(value);
        }

        public static bool IsNumber(string value)
        {
            if (value == null)
            {
                return false;
            }
            Regex regex = new Regex("^" + Number + "$");
            return regex.IsMatch(value);
        }

		public static string[] SplitKey(string compoundKey) {
			MatchCollection m = Regex.Matches(compoundKey, KeyOrArrayAccess);
			// Would prefer to use Linq for this, but MatchCollection does not seem to support it
			string[] result = new string[m.Count];
			for (int i = 0; i < result.Length; i++) {
				result[i] = m[i].Value;
			}
			return result;
		}
    }
}
