using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mustache
{
    public interface IArgument
    {
        string GetKey();

        object GetValue(Scope keyScope, Scope contextScope);
    }
}
