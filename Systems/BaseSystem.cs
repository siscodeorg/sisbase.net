using sisbase.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sisbase.Systems {
    public abstract class BaseSystem {
        public string Name;
        public string Description;
        public bool Enabled;

        public abstract Task Activate();
        public abstract Task Deactivate();
        public virtual async Task<bool> CheckPreconditions() => true;

        internal SystemData ToSystemData () => new(Name, Enabled);
    }
}
