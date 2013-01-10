using System;
using System.Collections.Generic;

namespace mustache
{
    /// <summary>
    /// Defines a tag that conditionally renders its content if preceding if and elif tags fail.
    /// </summary>
    internal sealed class ElifTagDefinition : ConditionTagDefinition
    {
        /// <summary>
        /// Initializes a new instance of an ElifTagDefinition.
        /// </summary>
        public ElifTagDefinition()
            : base("elif")
        {
        }
        
        /// <summary>
        /// Gets the tags that indicate the end of the current tags context.
        /// </summary>
        public override IEnumerable<TagDefinition> ClosingTags
        {
            get { return new TagDefinition[] { new IfTagDefinition() }; }
        }
    }
}
