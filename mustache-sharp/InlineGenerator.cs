using System;
using System.Collections.Generic;
using System.IO;

namespace Mustache
{
    /// <summary>
    /// Generates the text for a tag that is replaced with its generated text.
    /// </summary>
    internal sealed class InlineGenerator : IGenerator
    {
        private readonly TagDefinition _definition;
        private readonly ArgumentCollection _arguments;

        /// <summary>
        /// Initializes a new instance of an InlineGenerator.
        /// </summary>
        /// <param name="definition">The tag to render the text for.</param>
        /// <param name="arguments">The arguments passed to the tag.</param>
        public InlineGenerator(TagDefinition definition, ArgumentCollection arguments)
        {
            _definition = definition;
            _arguments = arguments;
        }

        void IGenerator.GetText(TextWriter writer, Scope scope, Scope context, Action<Substitution> postProcessor)
        {
            Dictionary<string, object> arguments;
            if (_definition.IsSetter)
            {
                arguments = _arguments.GetArgumentKeyNames();   
            }
            else
            {
                arguments = _arguments.GetArguments(scope, context);
            }            
            _definition.GetText(writer, arguments, context);
        }
    }
}
