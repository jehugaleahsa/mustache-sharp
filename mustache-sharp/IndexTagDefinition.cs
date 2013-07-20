using System;
using System.Collections.Generic;
using System.IO;

namespace Mustache
{
    /// <summary>
    /// Defines a tag that outputs the current index within an each loop.
    /// </summary>
    internal sealed class IndexTagDefinition : InlineTagDefinition
    {
        /// <summary>
        /// Initializes a new instance of an IndexTagDefinition.
        /// </summary>
        public IndexTagDefinition()
            : base("index")
        {
        }

        /// <summary>
        /// Gets whether the tag only exists within the scope of its parent.
        /// </summary>
        protected override bool GetIsContextSensitive()
        {
            return true;
        }

        /// <summary>
        /// Gets the text to output.
        /// </summary>
        /// <param name="writer">The writer to write the output to.</param>
        /// <param name="arguments">The arguments passed to the tag.</param>
        /// <param name="contextData">Extra data passed along with the context.</param>
        public override void GetText(TextWriter writer, Dictionary<string, object> arguments, object contextData)
        {
            int index = (int)contextData;
            writer.Write(index);
        }
    }
}
