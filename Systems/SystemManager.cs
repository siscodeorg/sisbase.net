using Discord.Commands;
using Discord.WebSocket;

using Microsoft.Extensions.DependencyInjection;

using sisbase.CommandsNext;
using sisbase.Common;
using sisbase.Configuration;
using sisbase.Logging;
using sisbase.Systems.Attributes;
using sisbase.Systems.Expansions;

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
        public ConcurrentDictionary<Type, BaseSystem> DisabledSystems { get; } = new();

        public SystemManager(DiscordSocketClient Client, SystemConfig Config, SisbaseCommandSystem CommandSystem) {
            client = Client;
            config = Config;
            commandSystem = CommandSystem;
        }

        public async Task<SisbaseResult> InstallSystemsAsync(Assembly assembly) {
            if (assemblyQueue.Contains(assembly))
                return SisbaseResult.FromSucess();

            assemblyQueue.Enqueue(assembly);
            await LoadAssemblyQueue();
            await ReloadCommandSystem();
            return SisbaseResult.FromSucess();
        }

        public async Task<SisbaseResult> InstallSystemAsync<T>() where T : BaseSystem {
            var type = typeof(T);
            var query = IsValidType(type);

            foreach (var result in query) {
                if (!result.IsSucess)
                    Logger.Error("SystemManager", result.Error);
            }

            if(query.All(x => x.IsSucess)) {
                return await LoadType(type);
            } else {
                return SisbaseResult.FromError(string.Join('\n', query.Select(c => c.Error)));
            }
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
            system.Expansions = new();
            return await LoadSystem(type, system);
        }

        internal async Task<SisbaseResult> LoadSystem(Type type, BaseSystem system) {
            var result = IsValidType(type);
            if (result.Any(x => !x.IsSucess))
                return SisbaseResult.FromError(string.Join('\n', result.Select(c => c.Error)));

            if (IsConfigDisabled(system)) {
                system.Enabled = false;
                DisabledSystems.AddOrUpdate(type, system, (type, oldValue) => system);
                return SisbaseResult.FromError($"{system} is disabled in config @ {config.Path}");
            }

            var depChecker = await CheckDependencies(system, new());
            if (!depChecker.IsSucess) {
                return depChecker;
            } else {
                Logger.Log("SystemManager",$"Dependencies Loaded for {system.GetSisbaseTypeName()}");
            }

            if (system.services != null)
                Inject(system);

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

            if (system.HasExpansion<Sheduler>()) {
                var sheduler = (Sheduler)system;

                var timer = GenerateTimer(sheduler);

                if (timers.TryAdd(system, timer)) {
                    Logger.Warn("SystemManager", $"Scheduler expansion loaded for {system.Name}.");
                } else {
                    Logger.Error("SystemManager", $"An error has occoured while loading the sheduler expansion on {system.Name}.");
                }

                system.Expansions.Add(sheduler);
            }

            LoadedSystems.TryAdd(type, system);
            return SisbaseResult.FromSucess();
        }

        internal async Task<SisbaseResult> UnloadSystem(Type type, BaseSystem system) {
            if (UnloadedSystems.ContainsKey(type))
                return SisbaseResult.FromSucess();

            if (!LoadedSystems.ContainsKey(type))
                return SisbaseResult.FromError($"{system} was not loaded");

            var check = IsValidType(type);
            if (check.Any(x => !x.IsSucess))
                return SisbaseResult.FromError(string.Join('\n', check.Select(c => c.Error)));

            if (system.HasExpansion<Sheduler>()) {
                timers[system].Dispose();
                if(timers.TryRemove(system, out var _)) {
                    Logger.Warn("SystemManager", $"Scheduler expansion unloaded for {system.Name}.");
                } else {
                    Logger.Error("SystemManager", $"An error has occoured while unloading the sheduler expansion on {system.Name}.");
                }
            }

            await system.Deactivate();

            if (!LoadedSystems.TryRemove(new(type, system)))
                return SisbaseResult.FromError($"Could not remove ({type},{system}) from LoadedSystems. Please report this to the sisbase devs.");

            if (!UnloadedSystems.TryAdd(type, system))
                return SisbaseResult.FromError($"Could not add ({type},{system}) to UnloadedSystems. Please report this to the sisbase devs.");

            return SisbaseResult.FromSucess();
        }

        internal async Task LoadAssemblyQueue() {
            while(assemblyQueue.TryPeek(out _)) {
                assemblyQueue.TryDequeue(out var assembly);
                Logger.Log("SystemManager", $"Loading systems from {assembly.GetName().Name}");

                var result = await LoadAssembly(assembly);

                if (!result.IsSucess) {
                    Logger.Error("SystemManager", result.Error);
                } else {
                    Logger.Log("SystemManager", $"Systems from {assembly.GetName().Name} loaded sucessfully.");
                    loadedAssemblies.Add(assembly);
                }
            }

            UpdateConfig();

            Logger.Log("SystemManager", "Finished loading all assemblies");
        }

        internal async Task RetryUnloadedSystems() {
            Logger.Log("SystemManager", "Retrying loading for all unloaded systems.");
            foreach (var (type, system) in UnloadedSystems) {
                Logger.Log("SystemManager", $"Loading {system}");
                var result = await LoadSystem(type, system);

                if(result.IsSucess) {
                    Logger.Log("SystemManager", $"Sucessfully loaded {system}");
                } else {
                    Logger.Error("SystemManager", result.Error);
                }
            }

            await ReloadCommandSystem();
            Logger.Log("SystemManager", "Retry attempt finished.");
        }

        internal void UpdateConfig() {
            Logger.Log("SystemManager", "Saving all systems to config file");

            var allSystems = LoadedSystems.Values.ToList();
            allSystems.AddRange(UnloadedSystems.Values);
            allSystems.AddRange(DisabledSystems.Values);

            var kvps = allSystems
                .Distinct()
                .Select(x =>
                    KeyValuePair.Create(
                        x.GetSisbaseTypeName(),
                        x.ToSystemData()
                ));

            config.Systems = kvps.ToDictionary(x => x.Key, y => y.Value);
            config.Update();

            Logger.Log("SystemManager", $"Config File @{config.Path} updated");
        }

        internal async Task<SisbaseResult> CheckDependencies(BaseSystem s, List<Type> Stack) {
            var deps = GetDependencies(s);
            if (!deps.Any()) return SisbaseResult.FromSucess();
            else {
                ServiceCollection collection = new();
                var intersection = deps.Intersect(Stack);
                if (intersection.Any()) return SisbaseResult.FromError(
                    $"Cyclical dependency detected while loading {s.GetSisbaseTypeName()}.\n" +
                    $"{s.GetSisbaseTypeName()} -> {string.Join(",",intersection)}\n" +
                    $"{string.Join(",", intersection)} -> {s.GetSisbaseTypeName()}"
                    );

                foreach(var dep in deps) {
                    var result = await LoadType(dep);

                    if (!result.IsSucess) return SisbaseResult.FromError(
                        $"Error while loading dependencies for {s.GetSisbaseTypeName()}.\n"
                        + $" Dependency {dep.Name} errored while loading. \n Error(s) : {result.Error}");

                    var sys = LoadedSystems[dep];
                    collection.AddSingleton(dep,sys);
                    var innerDep = GetDependencies(sys);
                    var newStack = Stack;
                    newStack.Add(s.GetType());

                    s.services = collection.BuildServiceProvider();
                    if (!innerDep.Any()) return SisbaseResult.FromSucess();
                    return await CheckDependencies(sys, newStack);
                }

                return SisbaseResult.FromSucess();
            }
        }

        internal List<Type> GetDependencies(BaseSystem s) {
            var attrib = s.GetType().GetCustomAttribute<DependsAttribute>();
            if(attrib == null) return new();
            return attrib.Systems;
        }

        internal void Inject(BaseSystem system) {
            var type = system.GetType().GetTypeInfo();
            var injectableProperties = ReflectionUtils.GetInjectableProperties(type);
            foreach (var property in injectableProperties) {
                var instance = system.services.GetService(property.PropertyType);
                if (instance == null) continue;
                property.SetValue(system, instance);
            }
        }

        internal async Task ReloadCommandSystem() {
            var modules = commandSystem._commandService.Modules;

            foreach (var module in modules) {
                await commandSystem._commandService.RemoveModuleAsync(module);
            }

            foreach (var assembly in loadedAssemblies)
                await commandSystem._commandService.AddModulesAsync(assembly, commandSystem._provider);
        }

        internal async Task<SisbaseResult> LoadAssembly(Assembly assembly) {
            if (loadedAssemblies.Contains(assembly))
                return SisbaseResult.FromError($"Assembly {assembly.GetName().Name} is already loaded.");

            var systemTypes = GetSystemsFromAssembly(assembly);
            foreach (var type in systemTypes) {
                var result =  await LoadType(type);
                if (result.IsSucess) {
                    Logger.Log("SystemManager", $"{type.Name} Loaded sucessfully.");
                } else {
                    Logger.Error("SystemManager", result.Error);
                }
            }
            return SisbaseResult.FromSucess();
        }

        internal static List<Type> GetSystemsFromAssembly(Assembly assembly)
            => assembly.GetTypes().Where(x => IsValidType(x).All(v => v.IsSucess)).ToList();

        internal BaseSystem InitalLoadType(Type type) {
            var System = (BaseSystem)Activator.CreateInstance(type);

            if (System is ClientSystem clientSystem) {
                clientSystem.Client = client;
            }

            return System;
        }

        internal static List<SisbaseResult> IsValidType(Type type) {
            var errors = new List<SisbaseResult>();

            if (!type.IsSubclassOf(typeof(BaseSystem)))
                errors.Add(SisbaseResult.FromError($"{type} is not a subclass of BaseSystem"));

            if (type.IsNotPublic)
                errors.Add(SisbaseResult.FromError($"{type} is not public"));

            if (type.IsAbstract)
                errors.Add(SisbaseResult.FromError($"{type} is abstract"));

            if (errors.Any())
                return errors;

            return new List<SisbaseResult> { SisbaseResult.FromSucess() };
        }

        internal Timer GenerateTimer(Sheduler sheduler) =>
            new Timer(new TimerCallback(x => sheduler.RunContinuously()), null, TimeSpan.FromSeconds(1), sheduler.Timeout);


        internal bool IsConfigDisabled(BaseSystem system) {
            if (config.Systems.ContainsKey(system.GetSisbaseTypeName())) {
                if (!config.Systems[system.GetSisbaseTypeName()].Enabled) {
                    return true;
                }
            }
            return false;
        }
    }
}
