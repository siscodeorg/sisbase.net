using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Discord.Commands;

namespace sisbase.Common {
    internal static class ReflectionUtils {

        private static readonly TypeInfo ObjectTypeInfo = typeof(object).GetTypeInfo();

        internal static PropertyInfo[] GetInjectableProperties(TypeInfo ownerType) {
            var result = new List<PropertyInfo>();
            while (ownerType != ObjectTypeInfo) {
                foreach (var prop in ownerType.DeclaredProperties) {
                    if (prop.SetMethod?.IsStatic == false &&
                        prop.SetMethod?.IsPublic == true &&
                        prop.GetCustomAttribute<DontInjectAttribute>() == null)
                        result.Add(prop);
                }
                ownerType = ownerType.BaseType.GetTypeInfo();
            }
            return result.ToArray();
        }
        
        internal static Stream GetResource(string resourceName)
            => typeof(SisbaseBot).Assembly.GetManifestResourceStream(resourceName);
    }
}
