namespace Mustache
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

        /// <summary>
        /// Gets whether the tag only exists within the scope of its parent.
        /// </summary>
        protected override bool GetIsContextSensitive()
        {
            return false;
        }
    }
}
