using System;
using System.Collections.Generic;
using System.IO;

namespace Mustache
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

        void IGenerator.GetText(TextWriter writer, Scope keyScope, Scope contextScope, Action<Substitution> postProcessor)
        {
            Dictionary<string, object> arguments = _arguments.GetArguments(keyScope, contextScope);
            IEnumerable<NestedContext> contexts = _definition.GetChildContext(writer, keyScope, arguments, contextScope);
            List<IGenerator> generators;
            if (_definition.ShouldGeneratePrimaryGroup(arguments))
            {
                generators = _primaryGenerators;
            }
            else
            {
                generators = new List<IGenerator>();
                if (_subGenerator != null)
                {
                    generators.Add(_subGenerator);
                }
            }
            foreach (NestedContext context in contexts)
            {
                foreach (IGenerator generator in generators)
                {
                    generator.GetText(context.Writer ?? writer, context.KeyScope ?? keyScope, context.ContextScope, postProcessor);
                }
                if (context.WriterNeedsConsidated)
                {
                    writer.Write(_definition.ConsolidateWriter(context.Writer ?? writer, arguments));
                }
            }
        }
    }
}