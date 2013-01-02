using System;
using System.Text;

namespace mustache
{
    internal sealed class WithBuilder : IBuilder
    {
        private readonly CompoundBuilder builder;

        public WithBuilder()
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
            Scope valueScope = scope.CreateChildScope(value);
            builder.Build(valueScope, output, provider);
        }
    }
}
