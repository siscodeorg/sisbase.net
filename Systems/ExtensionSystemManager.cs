using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using sisbase.Systems.Expansions;

namespace sisbase.Systems {
    public static class ExtensionSystemManager {
        public static string GetSisbaseTypeName(this BaseSystem system)
            => $"{system.GetType().Assembly.GetName().Name}::{system.GetType().FullName}";

        public static bool HasExpansion<T>(this BaseSystem system) where T : SystemExpansion
            => system.Expansions.Any(x => x is T);

    }
}
