using System;
using System.Collections.Generic;
using System.Text;

namespace mustache
{
    internal sealed class CompoundGenerator : IGenerator
    {
        private readonly List<IGenerator> _generators;

        public CompoundGenerator()
        {
            _generators = new List<IGenerator>();
        }

        public void AddGenerator(StaticGenerator generator)
        {
            _generators.Add(generator);
        }

        string IGenerator.GetText(object source)
        {
            StringBuilder builder = new StringBuilder();
            foreach (IGenerator generator in _generators)
            {
                builder.Append(generator.GetText(source));
            }
            string innerText = builder.ToString();
            // TODO - process with tag's custom handler
            return innerText;
        }
    }
}