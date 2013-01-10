using System;

namespace mustache
{
    /// <summary>
    /// Applies the values of an object to the format plan, generating a string.
    /// </summary>
    internal interface IGenerator
    {
        /// <summary>
        /// Generates the text when applying the format plan.
        /// </summary>
        /// <param name="provider">The format provider to use when formatting the keys.</param>
        /// <param name="scope">The current lexical scope of the keys.</param>
        /// <returns>The generated text.</returns>
        string GetText(IFormatProvider provider, KeyScope scope);
    }
}
