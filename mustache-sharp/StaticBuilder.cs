using System;
using System.Text;

namespace mustache
{
    internal sealed class StaticBuilder : IBuilder
    {
        public StaticBuilder()
        {
        }

        public string Value
        {
            get;
            set;
        }

        public void Build(Scope scope, StringBuilder output, IFormatProvider provider)
        {
            output.Append(Value);
        }
    }
}
