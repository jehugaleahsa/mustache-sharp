using System;
using System.Collections.Generic;
using System.Text;

namespace mustache
{
    internal sealed class CompoundBuilder : IBuilder
    {
        private readonly List<IBuilder> builders;

        public CompoundBuilder()
        {
            builders = new List<IBuilder>();
        }

        public void AddBuilder(IBuilder builder)
        {
            builders.Add(builder);
        }

        public void Build(Scope scope, StringBuilder output, IFormatProvider provider)
        {
            foreach (IBuilder builder in builders)
            {
                builder.Build(scope, output, provider);
            }
        }
    }
}
