using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace Mustache
{
    /// <summary>
    /// Provides utility methods that require regular expressions.
    /// </summary>
    public static class RegexHelper
    {
        internal const string Key = @"[_\w][_\w\d]*";
        internal const string CompoundKey = Key + @"(\." + Key + ")*";
		internal const string KeyOrArrayAccess = "(?:" + Key + @"|\[[^\]]+\]" + ")";
		internal const string CompoundKeyOrArrayAccess = KeyOrArrayAccess + @"(\." + KeyOrArrayAccess + ")*";

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
