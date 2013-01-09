using System;

namespace mustache
{
    /// <summary>
    /// Applies the values of an object to the format plan, generating a string.
    /// </summary>
    internal interface IGenerator
    {
        /// <summary>
        /// Generates the text when the values of the given object are applied to the format plan.
        /// </summary>
        /// <param name="source">The object whose values should be used to generate the text.</param>
        /// <returns>The generated text.</returns>
        string GetText(object source);
    }
}
