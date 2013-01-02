using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace mustache
{
    internal sealed class IfBuilder : IBuilder
    {
        private readonly CompoundBuilder trueBuilder;
        private readonly CompoundBuilder falseBuilder;

        public IfBuilder()
        {
            trueBuilder = new CompoundBuilder();
            falseBuilder = new CompoundBuilder();
        }

        public string Key
        {
            get;
            set;
        }

        public CompoundBuilder TrueBuilder
        {
            get { return trueBuilder; }
        }

        public CompoundBuilder FalseBuilder
        {
            get { return falseBuilder; }
        }

        public void Build(Scope scope, StringBuilder output, IFormatProvider provider)
        {
            object value = scope.Find(Key);
            bool truthyness = getTruthyness(value);
            if (truthyness)
            {
                trueBuilder.Build(scope, output, provider);
            }
            else
            {
                falseBuilder.Build(scope, output, provider);
            }
        }

        private bool getTruthyness(object value)
        {
            if (value == null)
            {
                return false;
            }
            IEnumerable enumerable = value as IEnumerable;
            if (enumerable != null)
            {
                return enumerable.Cast<object>().Any();
            }
            if (value is Char)
            {
                return (Char)value != '\0';
            }
            try
            {
                decimal number = (decimal)Convert.ChangeType(value, typeof(decimal));
                return number != 0.0m;
            }
            catch
            {
            }
            return true;
        }
    }
}
