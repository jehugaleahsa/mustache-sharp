using System;
using System.Collections.Generic;
using mustache.Properties;

namespace mustache
{
    /// <summary>
    /// Represents a scope of tags.
    /// </summary>
    internal sealed class TagScope
    {
        private readonly TagScope _parent;
        private readonly Dictionary<string, TagDefinition> _tagLookup;

        /// <summary>
        /// Initializes a new instance of a TagScope.
        /// </summary>
        public TagScope()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of a TagScope.
        /// </summary>
        /// <param name="parent">The parent scope to search for tag definitions.</param>
        public TagScope(TagScope parent)
        {
            _parent = parent;
            _tagLookup = new Dictionary<string, TagDefinition>();
        }

        /// <summary>
        /// Registers the tag in the current scope.
        /// </summary>
        /// <param name="definition">The tag to add to the current scope.</param>
        /// <exception cref="System.ArgumentException">The tag already exists at the current scope.</exception>
        public void AddTag(TagDefinition definition)
        {
            if (Find(definition.Name) != null)
            {
                string message = String.Format(Resources.DuplicateTagDefinition, definition.Name);
                throw new ArgumentException(Resources.DuplicateTagDefinition, "definition");
            }
            _tagLookup.Add(definition.Name, definition);
        }

        /// <summary>
        /// Trys to register the tag in the current scope.
        /// </summary>
        /// <param name="definition">The tag to add to the current scope.</param>
        public void TryAddTag(TagDefinition definition)
        {
            if (Find(definition.Name) == null)
            {
                _tagLookup.Add(definition.Name, definition);
            }
        }

        /// <summary>
        /// Finds the tag definition with the given name.
        /// </summary>
        /// <param name="tagName">The name of the tag definition to search for.</param>
        /// <returns>The tag definition with the name -or- null if it does not exist.</returns>
        public TagDefinition Find(string tagName)
        {
            TagDefinition definition;
            if (_tagLookup.TryGetValue(tagName, out definition))
            {
                return definition;
            }
            if (_parent == null)
            {
                return null;
            }
            return _parent.Find(tagName);
        }
    }
}
