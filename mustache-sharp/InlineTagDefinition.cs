using System;
using System.Collections.Generic;
using System.IO;

namespace mustache
{
    /// <summary>
    /// Defines a tag that cannot contain inner text.
    /// </summary>
    public abstract class InlineTagDefinition : TagDefinition
    {
        /// <summary>
        /// Initializes a new instance of an InlineTagDefinition.
        /// </summary>
        /// <param name="tagName">The name of the tag being defined.</param>
        protected InlineTagDefinition(string tagName)
            : base(tagName)
        {
        }

        /// <summary>
        /// Initializes a new instance of an InlineTagDefinition.
        /// </summary>
        /// <param name="tagName">The name of the tag being defined.</param>
        /// <param name="isBuiltin">Specifies whether the tag is a built-in tag.</param>
        internal InlineTagDefinition(string tagName, bool isBuiltin)
            : base(tagName, isBuiltin)
        {
        }

        /// <summary>
        /// Gets or sets whether the tag can have content.
        /// </summary>
        /// <returns>True if the tag can have a body; otherwise, false.</returns>
        protected override bool GetHasContent()
        {
            return false;
        }
    }
}
