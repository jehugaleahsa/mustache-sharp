using System;

namespace mustache
{
    internal sealed class TagAttributes
    {
        public TagAttributes()
        {
        }

        public TagType Type
        {
            get;
            set;
        }

        public bool IsOutput
        {
            get;
            set;
        }
    }
}
