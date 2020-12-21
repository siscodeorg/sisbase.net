using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sisbase.Systems.Expansions {
    public interface Sheduler : SystemExpansion {
        TimeSpan Timeout { get; }
        Action RunContinuously { get; }
    }
}
