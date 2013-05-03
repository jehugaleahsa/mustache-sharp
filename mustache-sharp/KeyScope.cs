using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Mustache.Properties;

namespace Mustache
{
    /// <summary>
    /// Represents a scope of keys.
    /// </summary>
    public sealed class KeyScope
    {
        private readonly object _source;
        private readonly KeyScope _parent;

        /// <summary>
        /// Initializes a new instance of a KeyScope.
        /// </summary>
        /// <param name="source">The object to search for keys in.</param>
        internal KeyScope(object source)
            : this(source, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of a KeyScope.
        /// </summary>
        /// <param name="source">The object to search for keys in.</param>
        /// <param name="parent">The parent scope to search in if the value is not found.</param>
        internal KeyScope(object source, KeyScope parent)
        {
            _parent = parent;
            _source = source;
        }

        /// <summary>
        /// Occurs when a key/property is found in the object graph.
        /// </summary>
        public event EventHandler<KeyFoundEventArgs> KeyFound;

        /// <summary>
        /// Occurs when a key/property is not found in the object graph.
        /// </summary>
        public event EventHandler<KeyNotFoundEventArgs> KeyNotFound;

        /// <summary>
        /// Creates a child scope that searches for keys in the given object.
        /// </summary>
        /// <param name="source">The object to search for keys in.</param>
        /// <returns>The new child scope.</returns>
        public KeyScope CreateChildScope(object source)
        {
            KeyScope scope = new KeyScope(source, this);
            scope.KeyFound = KeyFound;
            scope.KeyNotFound = KeyNotFound;
            return scope;
        }

        /// <summary>
        /// Attempts to find the value associated with the key with given name.
        /// </summary>
        /// <param name="name">The name of the key.</param>
        /// <returns>The value associated with the key with the given name.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">A key with the given name could not be found.</exception>
        internal object Find(string name)
        {
            string[] names = name.Split('.');
            string member = names[0];
            object nextLevel = _source;
            if (member != "this")
            {
                nextLevel = find(name, member);
            }
            for (int index = 1; index < names.Length; ++index)
            {
                IDictionary<string, object> context = toLookup(nextLevel);
                member = names[index];
                if (!context.TryGetValue(member, out nextLevel))
                {
                    nextLevel = handleKeyNotFound(name, member);
                }
            }
            if (KeyFound != null)
            {
                KeyFoundEventArgs args = new KeyFoundEventArgs(name, nextLevel);
                KeyFound(this, args);
                nextLevel = args.Substitute;
            }
            return nextLevel;
        }

        private object find(string fullName, string memberName)
        {
            IDictionary<string, object> lookup = toLookup(_source);
            if (lookup.ContainsKey(memberName))
            {
                return lookup[memberName];
            }
            if (_parent == null)
            {
                return handleKeyNotFound(fullName, memberName);
            }
            return _parent.find(fullName, memberName);
        }

        private object handleKeyNotFound(string fullName, string memberName)
        {
            KeyNotFoundEventArgs args = new KeyNotFoundEventArgs(fullName, memberName);
            if (KeyNotFound != null)
            {
                KeyNotFound(this, args);
            }
            if (args.Handled)
            {
                return args.Substitute;
            }
            string message = String.Format(CultureInfo.CurrentCulture, Resources.KeyNotFound, memberName);
            throw new KeyNotFoundException(message);
        }

        private static IDictionary<string, object> toLookup(object value)
        {
            IDictionary<string, object> lookup = value as IDictionary<string, object>;
            if (lookup == null)
            {
                lookup = new PropertyDictionary(value);
            }
            return lookup;
        }
    }
}
