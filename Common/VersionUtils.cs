using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sisbase.Common {
    public static class VersionUtils {
        public static string GetVersion() {
            var location = typeof(SisbaseBot).Assembly.Location;
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(location);
            return fileVersionInfo.FileVersion;
        }

        public static string GetFullVersion() {
            var location = typeof(SisbaseBot).Assembly.Location;
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(location);
            return fileVersionInfo.ProductVersion;
        }
    }
}
