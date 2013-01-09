using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using mustache.Properties;

namespace mustache
{
    /// <summary>
    /// Defines the attributes of a custom tag.
    /// </summary>
    public sealed class TagDefinition
    {
        private readonly string _tagName;
        private readonly List<TagParameter> _parameters;
        private readonly List<TagDefinition> _childTagDefinitions;
        private TagParameter _scopeParameter;

        /// <summary>
        /// Initializes a new instance of a TagDefinition.
        /// </summary>
        /// <param name="tagName">The name of the tag.</param>
        /// <exception cref="System.ArgumentException">The name of the tag is null or blank.</exception>
        public TagDefinition(string tagName)
        {
            if (!RegexHelper.IsValidIdentifier(tagName))
            {
                throw new ArgumentException(Resources.BlankTagName, "tagName");
            }
            _tagName = tagName;
            _parameters = new List<TagParameter>();
            _childTagDefinitions = new List<TagDefinition>();
        }

        /// <summary>
        /// Gets the name of the tag.
        /// </summary>
        public string Name
        {
            get { return _tagName; }
        }

        /// <summary>
        /// Specifies that the tag expects the given parameter information.
        /// </summary>
        /// <param name="parameter">The parameter to add.</param>
        /// <exception cref="System.ArgumentNullException">The parameter is null.</exception>
        /// <exception cref="System.ArgumentException">A parameter with the same name already exists.</exception>
        public void AddParameter(TagParameter parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException("parameter");
            }
            if (_parameters.Any(p => p.Name == parameter.Name))
            {
                throw new ArgumentException(Resources.DuplicateParameter, "parameter");
            }
            _parameters.Add(parameter);
            if (parameter.IsScopeContext)
            {
                _scopeParameter = parameter;
            }
        }

        /// <summary>
        /// Gets the parameters that are defined for the tag.
        /// </summary>
        public IEnumerable<TagParameter> Parameters
        {
            get { return new ReadOnlyCollection<TagParameter>(_parameters); }
        }

        /// <summary>
        /// Gets or sets whether the tag contains content.
        /// </summary>
        public bool HasBody
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether the tag defines a new scope based on an argument.
        /// </summary>
        public bool IsScoped
        {
            get;
            set;
        }

        /// <summary>
        /// Specifies that the given tag is in scope within the current tag.
        /// </summary>
        /// <param name="childTag">The tag that is in scope within the current tag.</param>
        public void AddChildTag(TagDefinition childTag)
        {
            if (childTag == null)
            {
                throw new ArgumentNullException("childTag");
            }
            _childTagDefinitions.Add(childTag);
        }

        /// <summary>
        /// Gets the tags that are in scope within the current tag.
        /// </summary>
        public IEnumerable<TagDefinition> ChildTags
        {
            get { return new ReadOnlyCollection<TagDefinition>(_childTagDefinitions); }
        }
    }
}
