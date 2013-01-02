using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace mustache
{
    internal sealed class EachBuilder : IBuilder
    {
        private readonly CompoundBuilder builder;

        public EachBuilder()
        {
            builder = new CompoundBuilder();
        }

        public string Key
        {
            get;
            set;
        }

        public CompoundBuilder Builder
        {
            get { return builder; }
        }

        public void Build(Scope scope, StringBuilder output, IFormatProvider provider)
        {
            object value = scope.Find(Key);
            IEnumerable enumerable = value as IEnumerable;
            if (enumerable == null)
            {
                return;
            }
            foreach (object item in enumerable)
            {
                IDictionary<string, object> lookup = item as IDictionary<string, object>;
                if (lookup == null)
                {
                    lookup = new PropertyDictionary(item);
                }
                Scope itemScope = scope.CreateChildScope(item);
                builder.Build(itemScope, output, provider);
            }
        }
    }
}
