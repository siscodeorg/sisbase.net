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
        internal DiscordSocketClient client { get; init; }
        internal SystemConfig config { get; init; }
        internal SisbaseCommandSystem commandSystem { get; init; }
        internal ConcurrentDictionary<BaseSystem, Timer> timers { get; } = new();
        internal ConcurrentQueue<Assembly> assemblyQueue { get; } = new();
        internal ConcurrentBag<Assembly> loadedAssemblies { get; } = new();

        public ConcurrentDictionary<Type, BaseSystem> LoadedSystems { get; } = new();
        public ConcurrentDictionary<Type, BaseSystem> UnloadedSystems { get; } = new();

        public SystemManager(DiscordSocketClient Client, SystemConfig Config, SisbaseCommandSystem CommandSystem) {
            client = Client;
            config = Config;
            commandSystem = CommandSystem;
        }

        internal async Task<SisbaseResult> LoadType(Type type) {
            if (LoadedSystems.ContainsKey(type))
                return SisbaseResult.FromSucess();

            BaseSystem system;

            if (UnloadedSystems.ContainsKey(type)) {
                system = UnloadedSystems[type];
            }
            else {
                system = InitalLoadType(type);
            }

            return await LoadSystem(type, system);
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
                await clientSystem.ApplyToClient(client);
            }

            LoadedSystems.TryAdd(type, system);
            return SisbaseResult.FromSucess();
        }

        internal BaseSystem InitalLoadType(Type type) {
            var System = (BaseSystem)Activator.CreateInstance(type);

            if (System is ClientSystem clientSystem) {
                clientSystem.Client = client;
            }

            return System;
        }
    }
}
