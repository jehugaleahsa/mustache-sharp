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
        private readonly LinkedList<IGenerator> _primaryGenerators;
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
            _primaryGenerators = new LinkedList<IGenerator>();
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

        /// <summary>
        /// Creates a StaticGenerator from the given value and adds it.
        /// </summary>
        /// <param name="generators">The static generators to add.</param>
        public void AddStaticGenerators(IEnumerable<StaticGenerator> generators)
        {
            foreach (StaticGenerator generator in generators)
            {
                LinkedListNode<IGenerator> node = _primaryGenerators.AddLast(generator);
                generator.Node = node;
            }
        }

        private void addGenerator(IGenerator generator, bool isSubGenerator)
        {
            if (isSubGenerator)
            {
                _subGenerator = generator;
            }
            else
            {
                _primaryGenerators.AddLast(generator);
            }
        }

        void IGenerator.GetText(KeyScope scope, TextWriter writer)
        {
            Dictionary<string, object> arguments = _arguments.GetArguments(scope);
            IEnumerable<NestedContext> contexts = _definition.GetChildContext(writer, scope, arguments);
            LinkedList<IGenerator> generators;
            if (_definition.ShouldGeneratePrimaryGroup(arguments))
            {
                generators = _primaryGenerators;
            }
            else
            {
                generators = new LinkedList<IGenerator>();
                if (_subGenerator != null)
                {
                    generators.AddLast(_subGenerator);
                }
            }
            foreach (NestedContext context in contexts)
            {
                foreach (IGenerator generator in generators)
                {
                    generator.GetText(context.KeyScope ?? scope, context.Writer ?? writer);
                    if (context.WriterNeedsConsidated)
                    {
                        writer.Write(_definition.ConsolidateWriter(context.Writer ?? writer, arguments));
                    }
                }
            }
        }
    }
}