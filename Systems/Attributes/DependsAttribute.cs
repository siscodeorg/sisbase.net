using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sisbase.Systems.Attributes {
    [AttributeUsage(AttributeTargets.Class)]
    public class DependsAttribute : Attribute {
        internal List<Type> Systems = new();
        public DependsAttribute(params Type[] systems) {
            foreach(var system in systems) {
                var result = SystemManager.IsValidType(system);
                if(result.Any(x => !x.IsSucess)) {
                    throw new InvalidCastException($"Type {system} isn't a valid system type. Reasons: \n" +
                        $"{string.Join("\n",result.Select(x => x.Error))}");
                }
                Systems.Add(system);
            }
        }
    }
}
