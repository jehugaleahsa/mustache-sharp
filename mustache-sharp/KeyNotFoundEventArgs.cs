using System;

namespace Mustache
{
    /// <summary>
    /// Holds the information needed to handle a missing key.
    /// </summary>
    public class KeyNotFoundEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of a KeyNotFoundEventArgs.
        /// </summary>
        /// <param name="key">The fully-qualified key.</param>
        /// <param name="missingMember">The part of the key that could not be found.</param>
        internal KeyNotFoundEventArgs(string key, string missingMember)
        {
            Key = key;
            MissingMember = missingMember;
        }

        /// <summary>
        /// Gets the fully-qualified key.
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Gets the part of the key that could not be found.
        /// </summary>
        public string MissingMember { get; private set; }

        /// <summary>
        /// Gets or sets whether to use the substitute.
        /// </summary>
        public bool Handled { get; set; }

        /// <summary>
        /// Gets or sets the object to use as the substitute.
        /// </summary>
        public object Substitute { get; set; }
    }
}
