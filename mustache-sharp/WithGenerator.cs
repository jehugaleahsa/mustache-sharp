using System;
using System.Collections.Generic;

namespace mustache
{
    /// <summary>
    /// Defines a tag that changes the scope to the object passed as an argument.
    /// </summary>
    internal sealed class WithTagDefinition : TagDefinition
    {
        private const string contextParameter = "context";

        /// <summary>
        /// Initializes a new instance of a WithTagDefinition.
        /// </summary>
        public WithTagDefinition()
            : base("with", true)
        {
        }

        /// <summary>
        /// Gets the parameters that can be passed to the tag.
        /// </summary>
        /// <returns>The parameters.</returns>
        protected override TagParameter[] GetParameters()
        {
            return new TagParameter[] { new TagParameter(contextParameter) { IsRequired = true } };
        }

        /// <summary>
        /// Gets whether the tag has content.
        /// </summary>
        public override bool HasBody
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the tags that come into scope within the tag.
        /// </summary>
        /// <returns>The child tag.</returns>
        protected override TagDefinition[] GetChildTags()
        {
            return new TagDefinition[0];
        }

        /// <summary>
        /// Gets the scopes to use for generating the tag's content.
        /// </summary>
        /// <param name="scope">The current scope.</param>
        /// <param name="arguments">The arguments that were passed to the tag.</param>
        /// <returns>The scopes to use for generating the tag's content.</returns>
        public override IEnumerable<KeyScope> GetChildScopes(KeyScope scope, Dictionary<string, object> arguments)
        {
            object context = arguments[contextParameter];
            yield return scope.CreateChildScope(context);
        }
    }
}
