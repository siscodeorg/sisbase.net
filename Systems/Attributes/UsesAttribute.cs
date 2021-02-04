using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sisbase.Systems.Attributes {
    [AttributeUsage(AttributeTargets.Class)]
    public class UsesAttribute : Attribute {
        internal List<Type> types;
        public UsesAttribute(params Type[] Types)
            => types = Types.ToList();
    }
}
