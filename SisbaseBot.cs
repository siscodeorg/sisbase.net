using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using sisbase.CommandsNext;
using sisbase.Configuration;
using sisbase.Systems;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace sisbase {
    public class SisbaseBot {
        public DiscordSocketClient Client { get; internal init;}
        public MainConfig Config { get; internal init; }
        public SisbaseCommandSystem CommandSystem { get; internal init; }
        internal PrefixResolver PrefixResolver { get; set; }
        public SystemManager Systems { get; internal set; }

        public SisbaseBot(DiscordSocketClient client, MainConfig config) {
            Client = client;
            Config = config;
            CommandSystem = new(Client);

            PrefixResolver = new RealTimePrefixResolver(CommandSystem, Config.Data.Prefixes.ToArray());
        }

        public SisbaseBot(DiscordSocketClient client, FileInfo configFile) {
            if (configFile.Extension != ".json") return;

            Client = client;

            Config = new();
            Config.Create(configFile);
            CommandSystem = new(Client);

            PrefixResolver = new RealTimePrefixResolver(CommandSystem, Config.Data.Prefixes.ToArray());
        }

        public void UsePrefixResolver(PrefixResolver resolver) {
            PrefixResolver = resolver;
        }

        public void UseSystemsApi(SystemConfig config) {
            Systems = new(Client, config, CommandSystem);
            CommandSystem._collection.AddSingleton(Systems);
            CommandSystem._provider = CommandSystem._collection.BuildServiceProvider();
        }

        public void WithServices(Action<IServiceCollection> Services) { 
            Services?.Invoke(CommandSystem._collection);
            Systems.WithServices(Services);
        }
        public Task InstallCommandsAsync(Assembly assembly)
            => CommandSystem.InstallCommandsAsync(assembly);

        public Task InstallSystemsAsync(Assembly assembly)
            => Systems.InstallSystemsAsync(assembly);

        public async Task StartAsync() {
            CommandSystem._prefixResolver = PrefixResolver;

            Client.Log += async (msg) => { Console.WriteLine(msg.ToString()); };
            await Client.LoginAsync(TokenType.Bot, Config.Data.Token);
            await Client.StartAsync();
            //Ugly Workaround until ConcurrencyUtils is ported.
            await Task.Delay(-1);
        }
    }
}
