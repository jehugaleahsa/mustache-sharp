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
    public sealed class Scope
    {
        private readonly object _source;
        private readonly Scope _parent;

        /// <summary>
        /// Initializes a new instance of a KeyScope.
        /// </summary>
        /// <param name="source">The object to search for keys in.</param>
        internal Scope(object source)
            : this(source, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of a KeyScope.
        /// </summary>
        /// <param name="source">The object to search for keys in.</param>
        /// <param name="parent">The parent scope to search in if the value is not found.</param>
        internal Scope(object source, Scope parent)
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
        /// Creates a child scope that searches for keys in a default dictionary of key/value pairs.
        /// </summary>
        /// <returns>The new child scope.</returns>
        public Scope CreateChildScope()
        {
            return CreateChildScope(new Dictionary<string, object>());
        }

        /// <summary>
        /// Creates a child scope that searches for keys in the given object.
        /// </summary>
        /// <param name="source">The object to search for keys in.</param>
        /// <returns>The new child scope.</returns>
        public Scope CreateChildScope(object source)
        {
            Scope scope = new Scope(source, this);
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
            string member = null;
            object value = null;
            if (tryFind(name, ref member, ref value))
            {
                onKeyFound(name, ref value);
                return value;
            }
            if (onKeyNotFound(name, member, ref value))
            {
                return value;
            }
            string message = String.Format(CultureInfo.CurrentCulture, Resources.KeyNotFound, member);
            throw new KeyNotFoundException(message);
        }

        private void onKeyFound(string name, ref object value)
        {
            if (KeyFound != null)
            {
                KeyFoundEventArgs args = new KeyFoundEventArgs(name, value);
                KeyFound(this, args);
                value = args.Substitute;
            }
        }

        private bool onKeyNotFound(string name, string member, ref object value)
        {
            if (KeyNotFound == null)
            {
                return false;
            }
            KeyNotFoundEventArgs args = new KeyNotFoundEventArgs(name, member);
            KeyNotFound(this, args);
            if (!args.Handled)
            {
                return false;
            }
            value = args.Substitute;
            return true;
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

        internal void Set(string key, object value)
        {
            IDictionary<string, object> lookup = toLookup(_source);
            lookup[key] = value;
        }

        public bool TryFind(string name, out object value)
        {
            string member = null;
            value = null;
            return tryFind(name, ref member, ref value); 
        }

        private bool tryFind(string name, ref string member, ref object value)
        {
            string[] names = name.Split('.');
            member = names[0];
            value = _source;
            if (member != "this")
            {
                if (!tryFindFirst(member, ref value))
                {
                    return false;
                }
            }
            for (int index = 1; index < names.Length; ++index)
            {
                IDictionary<string, object> context = toLookup(value);
                member = names[index];
                if (!context.TryGetValue(member, out value))
                {
                    value = null;
                    return false;
                }
            }
            return true;
        }

        private bool tryFindFirst(string member, ref object value)
        {
            IDictionary<string, object> lookup = toLookup(_source);
            if (lookup.ContainsKey(member))
            {
                value = lookup[member];
                return true;
            }
            if (_parent == null)
            {
                value = null;
                return false;
            }
            return _parent.tryFindFirst(member, ref value);
        }
    }
}
