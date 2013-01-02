using System;
using System.Text;

namespace mustache
{
    internal sealed class KeyBuilder : IBuilder
    {
        public KeyBuilder()
        {
        }

        public string Key
        {
            get;
            set;
        }

        public string Alignment
        {
            get;
            set;
        }

        public string Format
        {
            get;
            set;
        }

        public void Build(Scope scope, StringBuilder output, IFormatProvider provider)
        {
            object value = scope.Find(Key);
            StringBuilder format = new StringBuilder();
            format.Append("{");
            format.Append("0");
            if (!String.IsNullOrWhiteSpace(Alignment))
            {
                format.Append(",");
                format.Append(Alignment);
            }
            if (!String.IsNullOrWhiteSpace(Format))
            {
                format.Append(":");
                format.Append(Format);
            }
            format.Append("}");
            output.AppendFormat(provider, format.ToString(), value);
        }
    }
}
