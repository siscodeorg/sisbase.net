using Discord.WebSocket;
using sisbase.CommandsNext;
using sisbase.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace sisbase.Systems {
    public class SystemManager {
        internal DiscordSocketClient Client { get; init; }
        internal SystemConfig Config { get; init; }
        internal SisbaseCommandSystem CommandSystem { get; init; }
        internal Dictionary<BaseSystem, Timer> Timers { get; } = new();
        internal Queue<Assembly> AssemblyQueue { get; } = new();
        internal List<Assembly> LoadedAssemblies { get; } = new();

        public Dictionary<Type, BaseSystem> LoadedSystems { get; } = new();
        public Dictionary<Type, BaseSystem> UnloadedSystems { get; } = new();

        public SystemManager(DiscordSocketClient client, SystemConfig config, SisbaseCommandSystem commandSystem) {
            Client = client;
            Config = config;
            CommandSystem = commandSystem;
        }
    }
}
