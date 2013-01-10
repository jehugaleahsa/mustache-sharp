using System;

namespace mustache
{
    /// <summary>
    /// Defines a tag that renders its content depending on the truthyness
    /// of its argument, with optional elif and else nested tags.
    /// </summary>
    internal sealed class IfTagDefinition : ConditionTagDefinition
    {
        /// <summary>
        /// Initializes a new instance of a IfTagDefinition.
        /// </summary>
        public IfTagDefinition()
            : base("if")
        {
        }
    }
}
