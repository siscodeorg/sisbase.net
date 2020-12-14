using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sisbase.Systems {
    public static class ExtensionSystemManager {
        public static string GetSisbaseTypeName(this BaseSystem system)
            => $"{system.GetType().Assembly}::{system.GetType().Namespace}.{system.GetType().Name}";
    }
}
