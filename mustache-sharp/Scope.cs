using System;
using System.Collections.Generic;
using mustache.Properties;

namespace mustache
{
    internal sealed class Scope
    {
        private readonly object topLevel;
        private Scope parent;

        public Scope(object topLevel)
        {
            parent = null;
            this.topLevel = topLevel;
        }

        public Scope CreateChildScope(object topLevel)
        {
            Scope scope = new Scope(topLevel);
            scope.parent = this;
            return scope;
        }

        public object Find(string name)
        {
            string[] names = name.Split('.');
            string member = names[0];
            object nextLevel = topLevel;
            if (member != "this")
            {
                nextLevel = find(member);
            }
            for (int index = 1; index < names.Length; ++index)
            {
                IDictionary<string, object> context = toLookup(nextLevel);
                member = names[index];
                nextLevel = context[member];
            }
            return nextLevel;
        }

        private object find(string name)
        {
            IDictionary<string, object> lookup = toLookup(topLevel);
            if (lookup.ContainsKey(name))
            {
                return lookup[name];
            }
            if (parent == null)
            {
                string message = String.Format(Resources.KeyNotFound, name);
                throw new KeyNotFoundException(message);
            }
            return parent.find(name);
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
