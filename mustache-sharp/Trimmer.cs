using System;

namespace mustache
{
    internal sealed class Trimmer
    {
        private bool hasHeader;
        private bool hasFooter;
        private bool hasTag;
        private bool canTrim;

        public Trimmer()
        {
            hasTag = false;
            canTrim = true;
        }

        public void AddStaticBuilder(CompoundBuilder builder, TagAttributes attributes, string value)
        {
            string trimmed = value;
            int newline = value.IndexOf(Environment.NewLine);
            if (newline == -1)
            {
                canTrim &= String.IsNullOrWhiteSpace(value);
            }
            else
            {
                // finish processing the previous line
                if (canTrim && hasTag && (!hasHeader || !hasFooter))
                {
                    string lineEnd = trimmed.Substring(0, newline);
                    if (String.IsNullOrWhiteSpace(lineEnd))
                    {
                        trimmed = trimmed.Substring(newline + Environment.NewLine.Length);
                    }
                }
                // start processing the next line
                hasTag = false;
                hasHeader = false;
                hasFooter = false;
                int lastNewline = value.LastIndexOf(Environment.NewLine);
                string lineStart = value.Substring(lastNewline + Environment.NewLine.Length);
                canTrim = String.IsNullOrWhiteSpace(lineStart);
            }
            hasTag |= attributes.Type != TagType.None;
            hasHeader |= attributes.Type == TagType.Header;
            hasFooter |= hasHeader && attributes.Type == TagType.Footer;
            canTrim &= !attributes.IsOutput;
            if (trimmed.Length > 0)
            {
                StaticBuilder leading = new StaticBuilder();
                leading.Value = trimmed;
                builder.AddBuilder(leading);
            }
        }
    }
}
