using System;
using System.Collections.Generic;

namespace Mustache
{
    /// <summary>
    /// Defines a tag that renders its content if all preceding if and elif tags.
    /// </summary>
    internal sealed class ElseTagDefinition : ContentTagDefinition
    {
        /// <summary>
        /// Initializes a new instance of a ElseTagDefinition.
        /// </summary>
        public ElseTagDefinition()
            : base("else", true)
        {
        }

        /// <summary>
        /// Gets whether the tag only exists within the scope of its parent.
        /// </summary>
        protected override bool GetIsContextSensitive()
        {
            return true;
        }

        /// <summary>
        /// Gets the tags that indicate the end of the current tag's content.
        /// </summary>
        protected override IEnumerable<string> GetClosingTags()
        {
            return new string[] { "if" };
        }

        /// <summary>
        /// Gets the parameters that are used to create a new child context.
        /// </summary>
        /// <returns>The parameters that are used to create a new child context.</returns>
        public override IEnumerable<TagParameter> GetChildContextParameters()
        {
            return new TagParameter[0];
        }
    }
}
