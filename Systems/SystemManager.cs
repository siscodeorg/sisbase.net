using Discord.WebSocket;
using sisbase.CommandsNext;
using sisbase.Common;
using sisbase.Configuration;
using System;
using System.Collections.Concurrent;
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
        internal ConcurrentDictionary<BaseSystem, Timer> Timers { get; } = new();
        internal ConcurrentQueue<Assembly> AssemblyQueue { get; } = new();
        internal ConcurrentBag<Assembly> LoadedAssemblies { get; } = new();

        public ConcurrentDictionary<Type, BaseSystem> LoadedSystems { get; } = new();
        public ConcurrentDictionary<Type, BaseSystem> UnloadedSystems { get; } = new();

        public SystemManager(DiscordSocketClient client, SystemConfig config, SisbaseCommandSystem commandSystem) {
            Client = client;
            Config = config;
            CommandSystem = commandSystem;
        }

        internal async Task<SisbaseResult> LoadSystem(Type type, BaseSystem system) {
            if (!type.IsSubclassOf(typeof(BaseSystem)))
                return SisbaseResult.FromError($"{type} is not a subclass of BaseSystem");

            if (!await system.CheckPreconditions()) {
                UnloadedSystems.AddOrUpdate(type, system, (type, oldValue) => system);
                return SisbaseResult.FromError($"Preconditions failed for {system}");
            }

            if (UnloadedSystems.ContainsKey(type))
                UnloadedSystems.TryRemove(new(type, UnloadedSystems[type]));

            await system.Activate();

            if (system is ClientSystem clientSystem) {
                await clientSystem.ApplyToClient(Client);
            }

            LoadedSystems.TryAdd(type, system);
            return SisbaseResult.FromSucess();
        }

        internal BaseSystem InitalLoadType(Type type) {
            var System = (BaseSystem)Activator.CreateInstance(type);

            if (System is ClientSystem clientSystem) {
                clientSystem.Client = Client;
            }

            return System;
        }
    }
}
