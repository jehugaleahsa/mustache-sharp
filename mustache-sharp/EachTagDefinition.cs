using System;
using System.Collections;
using System.Collections.Generic;

namespace mustache
{
    /// <summary>
    /// Defines a tag that can iterate over a collection of items and render
    /// the content using each item as the context.
    /// </summary>
    internal sealed class EachTagDefinition : ContentTagDefinition
    {
        private const string collectionParameter = "collection";

        /// <summary>
        /// Initializes a new instance of an EachTagDefinition.
        /// </summary>
        public EachTagDefinition()
            : base("each", true)
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
            return new TagParameter[] { new TagParameter(collectionParameter) { IsRequired = true } };
        }

        /// <summary>
        /// Gets the scopes for each of the items found in the argument.
        /// </summary>
        /// <param name="scope">The current scope.</param>
        /// <param name="arguments">The arguments passed to the tag.</param>
        /// <returns>The scopes for each of the items found in the argument.</returns>
        public override IEnumerable<KeyScope> GetChildScopes(KeyScope scope, Dictionary<string, object> arguments)
        {
            object value = arguments[collectionParameter];
            IEnumerable enumerable = value as IEnumerable;
            if (enumerable == null)
            {
                yield break;
            }
            foreach (object item in enumerable)
            {
                yield return scope.CreateChildScope(item);
            }
        }

        /// <summary>
        /// Gets the tags that are in scope under this tag.
        /// </summary>
        /// <returns>The name of the tags that are in scope.</returns>
        protected override IEnumerable<string> GetChildTags()
        {
            return new string[] { };
        }
    }
}
