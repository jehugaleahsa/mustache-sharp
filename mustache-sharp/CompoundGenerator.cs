using System;
using System.Collections.Generic;
using System.Text;

namespace mustache
{
    /// <summary>
    /// Builds text by combining the output of other generators.
    /// </summary>
    internal sealed class CompoundGenerator : IGenerator
    {
        private readonly TagDefinition _definition;
        private readonly ArgumentCollection _arguments;
        private readonly List<IGenerator> _primaryGenerators;
        private IGenerator _subGenerator;

        /// <summary>
        /// Initializes a new instance of a CompoundGenerator.
        /// </summary>
        /// <param name="definition">The tag that the text is being generated for.</param>
        /// <param name="arguments">The arguments that were passed to the tag.</param>
        public CompoundGenerator(TagDefinition definition, ArgumentCollection arguments)
        {
            _definition = definition;
            _arguments = arguments;
            _primaryGenerators = new List<IGenerator>();
        }

        /// <summary>
        /// Adds the given generator. 
        /// </summary>
        /// <param name="generator">The generator to add.</param>
        public void AddGenerator(IGenerator generator)
        {
            addGenerator(generator, false);
        }

        /// <summary>
        /// Adds the given generator, determining whether the generator should
        /// be part of the primary generators or added as an secondary generator.
        /// </summary>
        /// <param name="definition">The tag that the generator is generating text for.</param>
        /// <param name="generator">The generator to add.</param>
        public void AddGenerator(TagDefinition definition, IGenerator generator)
        {
            bool isSubGenerator = _definition.ShouldCreateSecondaryGroup(definition);
            addGenerator(generator, isSubGenerator);
        }

        private void addGenerator(IGenerator generator, bool isSubGenerator)
        {
            if (isSubGenerator)
            {
                _subGenerator = generator;
            }
            else
            {
                _primaryGenerators.Add(generator);
            }
        }

        string IGenerator.GetText(IFormatProvider provider, KeyScope scope)
        {
            StringBuilder builder = new StringBuilder();
            Dictionary<string, object> arguments = _arguments.GetArguments(scope);
            IEnumerable<KeyScope> scopes = _definition.GetChildScopes(scope, arguments);
            List<IGenerator> generators;
            if (_definition.ShouldGeneratePrimaryGroup(arguments))
            {
                generators = _primaryGenerators;
            }
            else
            {
                generators = new List<IGenerator>() { _subGenerator };
            }
            foreach (KeyScope childScope in scopes)
            {
                foreach (IGenerator generator in generators)
                {
                    builder.Append(generator.GetText(provider, childScope));
                }
            }
            string innerText = builder.ToString();
            string outerText = _definition.Decorate(provider, innerText, arguments);
            return outerText;
        }
    }
}