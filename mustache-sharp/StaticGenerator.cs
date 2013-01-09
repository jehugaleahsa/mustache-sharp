using System;

namespace mustache
{
    internal sealed class StaticGenerator : IGenerator
    {
        private readonly string _value;

        public StaticGenerator(string value)
        {
            _value = value;
        }

        string IGenerator.GetText(object source)
        {
            return _value;
        }
    }
}
