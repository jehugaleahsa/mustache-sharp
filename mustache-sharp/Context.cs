using System;

namespace mustache
{
    /// <summary>
    /// Represents a context within a template.
    /// </summary>
    public sealed class Context
    {
        /// <summary>
        /// Initializes a new instance of a Context.
        /// </summary>
        /// <param name="definition">The definition of tag that created the context.</param>
        /// <param name="argument">The argument used to create the context.</param>
        internal Context(TagDefinition definition, string argument)
        {
            Tag = definition;
            Argument = argument;
        }

        /// <summary>
        /// Gets the tag that created the context.
        /// </summary>
        public TagDefinition Tag { get; private set; }

        /// <summary>
        /// Gets the argument used to create the context.
        /// </summary>
        public string Argument { get; private set; }
    }
}
