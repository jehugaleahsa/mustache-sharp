using System;
using System.Collections.Generic;

namespace mustache
{
    /// <summary>
    /// Defines a tag that changes the scope to the object passed as an argument.
    /// </summary>
    internal sealed class WithTagDefinition : ContentTagDefinition
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
        /// Gets whether the tag only exists within the scope of its parent.
        /// </summary>
        protected override bool GetIsContextSensitive()
        {
            return false;
        }

        /// <summary>
        /// Gets the parameters that can be passed to the tag.
        /// </summary>
        /// <returns>The parameters.</returns>
        protected override IEnumerable<TagParameter> GetParameters()
        {
            return new TagParameter[] { new TagParameter(contextParameter) { IsRequired = true } };
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
