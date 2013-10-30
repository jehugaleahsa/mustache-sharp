using System;
using Mustache.Properties;

namespace Mustache
{
    /// <summary>
    /// Holds the information descibing a variable that is found in a template.
    /// </summary>
    public class VariableFoundEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of a VariableFoundEventArgs.
        /// </summary>
        /// <param name="key">The key that was found.</param>
        /// <param name="alignment">The alignment that will be applied to the substitute value.</param>
        /// <param name="formatting">The formatting that will be applied to the substitute value.</param>
        /// <param name="context">The context where the placeholder was found.</param>
        internal VariableFoundEventArgs(string name, string alignment, string formatting, Context[] context)
        {
            Name = name;
            Alignment = alignment;
            Formatting = formatting;
            Context = context;
        }

        /// <summary>
        /// Gets or sets the key that was found.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the alignment that will be applied to the substitute value.
        /// </summary>
        public string Alignment { get; set; }

        /// <summary>
        /// Gets or sets the formatting that will be applied to the substitute value.
        /// </summary>
        public string Formatting { get; set; }

        /// <summary>
        /// Gets the context where the placeholder was found.
        /// </summary>
        public Context[] Context { get; private set; }
    }
}
