using System;
using System.IO;

namespace Mustache
{
    /// <summary>
    /// Applies the values of an object to the format plan, generating a string.
    /// </summary>
    internal interface IGenerator
    {
        /// <summary>
        /// Generates the text when applying the format plan.
        /// </summary>
        /// <param name="keyScope">The current lexical scope of the keys.</param>
        /// <param name="writer">The text writer to send all text to.</param>
        /// <param name="contextScope">The data associated to the context.</param>
        /// <returns>The generated text.</returns>
        void GetText(Scope keyScope, TextWriter writer, Scope contextScope);
    }
}
