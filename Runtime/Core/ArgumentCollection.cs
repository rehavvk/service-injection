using System;
using System.Collections.Generic;
using System.Linq;

namespace Rehawk.ServiceInjection
{
    public class ArgumentCollection
    {
        private readonly List<object> arguments;
        
        public ArgumentCollection(object[] arguments)
        {
            this.arguments = new List<object>();
            if (arguments != null)
            {
                this.arguments.AddRange(arguments);
            }
        }

        public bool TryResolve(Type type, out object result)
        {
            result = arguments.FirstOrDefault(type.IsInstanceOfType);
            return result != null;
        }
    }
}