using System.Collections.Generic;
using System.IO;

namespace Mustache
{
    internal sealed class DelimiterTagDefinition : ContentTagDefinition
    {
        /// <summary>
        /// Initializes a new instance of an DelimiterTagDefinition.
        /// </summary>
        public DelimiterTagDefinition()
            : base("delimiter", true)
        {
        }

        /// <summary>
        /// Gets the parameters that are used to create a new child context.
        /// </summary>
        /// <returns>The parameters that are used to create a new child context.</returns>
        public override IEnumerable<TagParameter> GetChildContextParameters()
        {
            return new TagParameter[0];
        }

        /// <summary>
        /// Gets the context to use when building the inner text of the tag.
        /// </summary>
        /// <param name="writer">The text writer passed</param>
        /// <param name="keyScope">The current key scope.</param>
        /// <param name="arguments">The arguments passed to the tag.</param>
        /// <returns>The scope to use when building the inner text of the tag.</returns>
        public override IEnumerable<NestedContext> GetChildContext(
            TextWriter writer,
            Scope keyScope,
            Dictionary<string, object> arguments,
            Scope contextScope)
        {
            if (!(bool) contextScope.Find("isDelimited", false))
                yield break;

            foreach(var childContext in base.GetChildContext(writer, keyScope, arguments, contextScope))
            {
                yield return childContext;
            }
        }
    }
}
