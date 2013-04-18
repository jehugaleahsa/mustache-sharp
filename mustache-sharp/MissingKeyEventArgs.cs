using System;

namespace mustache
{
    /// <summary>
    /// Holds the information needed to handle a missing key.
    /// </summary>
    public class MissingKeyEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of a MissingKeyEventArgs.
        /// </summary>
        /// <param name="missingKey">The key that had no match.</param>
        internal MissingKeyEventArgs(string missingKey)
        {
            MissingKey = missingKey;
        }

        /// <summary>
        /// Gets the key that could not be found.
        /// </summary>
        public string MissingKey { get; private set; }

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
