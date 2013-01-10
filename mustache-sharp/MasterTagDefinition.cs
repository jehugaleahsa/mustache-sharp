using System;
using System.Collections.Generic;

namespace mustache
{
    /// <summary>
    /// Defines a pseudo tag that wraps the entire content of a format string.
    /// </summary>
    internal sealed class MasterTagDefinition : TagDefinition
    {
        /// <summary>
        /// Initializes a new instance of a MasterTagDefinition.
        /// </summary>
        public MasterTagDefinition()
            : base(String.Empty, true)
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
        /// Gets whether the tag has content.
        /// </summary>
        public override bool HasBody
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the tags that indicate the end of the tags context.
        /// </summary>
        public override IEnumerable<TagDefinition> ClosingTags
        {
            get { return new TagDefinition[0]; }
        }

        /// <summary>
        /// Gets the tags that come into scope within the context of the tag.
        /// </summary>
        /// <returns>The tags.</returns>
        protected override TagDefinition[] GetChildTags()
        {
            return new TagDefinition[]
            {
                new IfTagDefinition(),
                new EachTagDefinition(),
                new WithTagDefinition(),
            };
        }
    }
}
