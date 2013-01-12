using System;
using System.Collections.Generic;

namespace mustache
{
    /// <summary>
    /// Generates a static block of text.
    /// </summary>
    internal sealed class StaticGenerator : IGenerator
    {
        /// <summary>
        /// Initializes a new instance of a StaticGenerator.
        /// </summary>
        public StaticGenerator()
        {
        }

        /// <summary>
        /// Gets or sets the linked list node containing the current generator.
        /// </summary>
        public LinkedListNode<IGenerator> Node
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the static text.
        /// </summary>
        public string Value
        {
            get;
            set;
        }

        /// <summary>
        /// Removes the static text from the final output.
        /// </summary>
        public void Prune()
        {
            if (Node != null)
            {
                Node.List.Remove(Node);
                Node = null;
            }
        }

        string IGenerator.GetText(IFormatProvider provider, KeyScope scope)
        {
            return Value;
        }
    }
}
