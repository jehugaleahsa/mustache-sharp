using System;
using System.Collections.Generic;

namespace mustache
{
    /// <summary>
    /// Defines a tag that renders its content if all preceding if and elif tags.
    /// </summary>
    internal sealed class ElseTagDefinition : TagDefinition
    {
        /// <summary>
        /// Initializes a new instance of a ElseTagDefinition.
        /// </summary>
        public ElseTagDefinition()
            : base("else", true)
        {
        }

        /// <summary>
        /// Gets the parameters that can be passed to the tag.
        /// </summary>
        /// <returns>The parameters.</returns>
        protected override TagParameter[] GetParameters()
        {
            return new TagParameter[0];
        }

        /// <summary>
        /// Gets whether the tag contains content.
        /// </summary>
        public override bool HasBody
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the tags that indicate the end of the current tag's content.
        /// </summary>
        public override IEnumerable<TagDefinition> ClosingTags
        {
            get { return new TagDefinition[] { new IfTagDefinition() }; }
        }

        /// <summary>
        /// Gets the tags that come into scope within the context of the tag.
        /// </summary>
        /// <returns>The tag definitions.</returns>
        protected override TagDefinition[] GetChildTags()
        {
            return new TagDefinition[0];
        }
    }
}
