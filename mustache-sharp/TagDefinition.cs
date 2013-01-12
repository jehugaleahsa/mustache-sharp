using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using mustache.Properties;

namespace mustache
{
    /// <summary>
    /// Defines the attributes of a custom tag.
    /// </summary>
    public abstract class TagDefinition
    {
        private readonly string _tagName;

        /// <summary>
        /// Initializes a new instance of a TagDefinition.
        /// </summary>
        /// <param name="tagName">The name of the tag.</param>
        /// <exception cref="System.ArgumentException">The name of the tag is null or blank.</exception>
        protected TagDefinition(string tagName)
            : this(tagName, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of a TagDefinition.
        /// </summary>
        /// <param name="tagName">The name of the tag.</param>
        /// <param name="isBuiltIn">Specifies whether the tag is built-in or not. Checks are not performed on the names of built-in tags.</param>
        internal TagDefinition(string tagName, bool isBuiltIn)
        {
            if (!isBuiltIn && !RegexHelper.IsValidIdentifier(tagName))
            {
                throw new ArgumentException(Resources.BlankTagName, "tagName");
            }
            _tagName = tagName;
        }

        /// <summary>
        /// Gets the name of the tag.
        /// </summary>
        public string Name
        {
            get { return _tagName; }
        }

        /// <summary>
        /// Gets whether the tag is limited to the parent tag's context.
        /// </summary>
        internal bool IsContextSensitive
        {
            get { return GetIsContextSensitive(); }
        }

        /// <summary>
        /// Gets whether a tag is limited to the parent tag's context.
        /// </summary>
        protected abstract bool GetIsContextSensitive();

        /// <summary>
        /// Gets the parameters that are defined for the tag.
        /// </summary>
        internal IEnumerable<TagParameter> Parameters
        {
            get { return GetParameters(); }
        }

        /// <summary>
        /// Specifies which parameters are passed to the tag.
        /// </summary>
        /// <returns>The tag parameters.</returns>
        protected virtual IEnumerable<TagParameter> GetParameters()
        {
            return new TagParameter[] { };
        }

        /// <summary>
        /// Gets whether the tag contains content.
        /// </summary>
        internal bool HasContent
        {
            get { return GetHasContent(); }
        }

        /// <summary>
        /// Gets whether tag has content.
        /// </summary>
        /// <returns>True if the tag has content; otherwise, false.</returns>
        protected abstract bool GetHasContent();

        /// <summary>
        /// Gets the tags that can indicate that the tag has closed.
        /// This field is only used if no closing tag is expected.
        /// </summary>
        internal IEnumerable<string> ClosingTags
        {
            get  { return GetClosingTags(); }
        }

        protected virtual IEnumerable<string> GetClosingTags()
        {
            if (HasContent)
            {
                return new string[] { Name };
            }
            else
            {
                return new string[] { };
            }
        }

        /// <summary>
        /// Gets the tags that are in scope within the current tag.
        /// </summary>
        internal IEnumerable<string> ChildTags
        {
            get { return GetChildTags(); }
        }

        /// <summary>
        /// Specifies which tags are scoped under the current tag.
        /// </summary>
        /// <returns>The child tag definitions.</returns>
        protected virtual IEnumerable<string> GetChildTags()
        {
            return new string[] { };
        }

        /// <summary>
        /// Gets the scope to use when building the inner text of the tag.
        /// </summary>
        /// <param name="scope">The current scope.</param>
        /// <param name="arguments">The arguments passed to the tag.</param>
        /// <returns>The scope to use when building the inner text of the tag.</returns>
        public virtual IEnumerable<KeyScope> GetChildScopes(KeyScope scope, Dictionary<string, object> arguments)
        {
            yield return scope;
        }

        /// <summary>
        /// Applies additional formatting to the inner text of the tag.
        /// </summary>
        /// <param name="provider">The format provider to use.</param>
        /// <param name="innerText">The inner text of the tag.</param>
        /// <param name="arguments">The arguments passed to the tag.</param>
        /// <returns>The decorated inner text.</returns>
        public virtual string Decorate(IFormatProvider provider, string innerText, Dictionary<string, object> arguments)
        {
            return innerText;
        }

        /// <summary>
        /// Requests which generator group to associate the given tag type.
        /// </summary>
        /// <param name="definition">The child tag definition being grouped.</param>
        /// <returns>The name of the group to associate the given tag with.</returns>
        public virtual bool ShouldCreateSecondaryGroup(TagDefinition definition)
        {
            return false;
        }

        /// <summary>
        /// Gets whether the group with the given name should have text generated for them.
        /// </summary>
        /// <param name="arguments">The arguments passed to the tag.</param>
        /// <returns>True if text should be generated for the group; otherwise, false.</returns>
        public virtual bool ShouldGeneratePrimaryGroup(Dictionary<string, object> arguments)
        {
            return true;
        }
    }
}
