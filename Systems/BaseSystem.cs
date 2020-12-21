using sisbase.Configuration;
using sisbase.Systems.Expansions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sisbase.Systems {
    /// <summary>
    /// Class that all systems derive from. <br></br>
    /// Contains basic utilities common to all systems. <br></br>
    /// For a system to be loaded by the SystemManager, it must be public and inherit BaseSystem or any of its superclasses.
    /// </summary>
    public abstract class BaseSystem {
        /// <summary>
        /// Name of the system.
        /// </summary>
        public string Name;
        /// <summary>
        /// (Optional) A brief description of what this system does.
        /// </summary>
        public string Description;
        /// <summary>
        /// If a system is enabled, the SystemManager will attempt to load it. Otherwise it will be skipped. <br></br>
        /// This flag can be set on the generated config file or via the provided commands, if those are enabled.
        /// </summary>
        public bool Enabled = true;

        /// <summary>
        /// All expansions loaded on the system
        /// </summary>
        public List<SystemExpansion> Expansions { get; internal set; }

        /// <summary>
        /// A Task that will be executed after the preconditions pass, if <see cref="CheckPreconditions"/> is overriten by the user.
        /// </summary>
        public abstract Task Activate();
        /// <summary>
        /// A Task that will be executed once a system is deactivated.<br></br>
        /// A system can be deactivated via the provided commands, if those are enabled.
        /// </summary>
        public abstract Task Deactivate();

        /// <summary>
        /// A Task that will be executed at the start of the systems' lifecycle. <br></br>
        /// If it passes the system is enabled, otherwise the system is stored until another registering attempt is called.
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> CheckPreconditions() => true;

        internal SystemData ToSystemData () => new(Name, Enabled);
    }
}
