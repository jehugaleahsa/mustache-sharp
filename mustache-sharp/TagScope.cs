using System;
using System.Collections.Generic;

namespace mustache
{
    internal sealed class TagScope
    {
        private readonly TagScope _parent;
        private readonly Dictionary<string, TagDefinition> _tagLookup;

        public TagScope()
            : this(null)
        {
        }

        public TagScope(TagScope parent)
        {
            _parent = parent;
            _tagLookup = new Dictionary<string, TagDefinition>();
        }

        public void AddTag(TagDefinition tagDefinition)
        {
            _tagLookup.Add(tagDefinition.Name, tagDefinition);
        }

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
