using System;
using System.Text;

namespace mustache
{
    internal interface IBuilder
    {
        void Build(Scope scope, StringBuilder output, IFormatProvider provider);
    }
}
