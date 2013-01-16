using System;
using System.Collections.Generic;
using System.IO;

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
        /// Gets the context to use when building the inner text of the tag.
        /// </summary>
        /// <param name="writer">The text writer passed</param>
        /// <param name="scope">The current scope.</param>
        /// <param name="arguments">The arguments passed to the tag.</param>
        /// <returns>The scope to use when building the inner text of the tag.</returns>
        public override IEnumerable<NestedContext> GetChildContext(TextWriter writer, KeyScope scope, Dictionary<string, object> arguments)
        {
            object context = arguments[contextParameter];
            yield return new NestedContext() { KeyScope = scope.CreateChildScope(context), Writer = writer };
        }
    }
}
