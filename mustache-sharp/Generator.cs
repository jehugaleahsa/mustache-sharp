using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace mustache
{
    /// <summary>
    /// Generates text by substituting an object's values for placeholders.
    /// </summary>
    public sealed class Generator
    {
        private readonly IGenerator _generator;
        private readonly List<EventHandler<MissingKeyEventArgs>> _handlers;

        /// <summary>
        /// Initializes a new instance of a Generator.
        /// </summary>
        /// <param name="generator">The text generator to wrap.</param>
        internal Generator(IGenerator generator)
        {
            _generator = generator;
            _handlers = new List<EventHandler<MissingKeyEventArgs>>();
        }

        /// <summary>
        /// Occurs when a key/property is not found in the object graph.
        /// </summary>
        public event EventHandler<MissingKeyEventArgs> KeyNotFound
        {
            add { _handlers.Add(value); }
            remove { _handlers.Remove(value); }
        }

        /// <summary>
        /// Gets the text that is generated for the given object.
        /// </summary>
        /// <param name="source">The object to generate the text with.</param>
        /// <returns>The text generated for the given object.</returns>
        public string Render(object source)
        {
            return render(CultureInfo.CurrentCulture, source);
        }

        /// <summary>
        /// Gets the text that is generated for the given object.
        /// </summary>
        /// <param name="provider">The format provider to use.</param>
        /// <param name="source">The object to generate the text with.</param>
        /// <returns>The text generated for the given object.</returns>
        public string Render(IFormatProvider provider, object source)
        {
            if (provider == null)
            {
                provider = CultureInfo.CurrentCulture;
            }
            return render(provider, source);
        }

        private string render(IFormatProvider provider, object source)
        {
            KeyScope scope = new KeyScope(source);
            foreach (EventHandler<MissingKeyEventArgs> handler in _handlers)
            {
                scope.KeyNotFound += handler;
            }
            StringWriter writer = new StringWriter(provider);
            _generator.GetText(scope, writer);
            return writer.ToString();
        }
    }
}
