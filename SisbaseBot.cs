using Discord;
using Discord.WebSocket;
using sisbase.CommandsNext;
using sisbase.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sisbase {
    public class SisbaseBot {
        public DiscordSocketClient Client { get; internal init;}
        public MainConfig Config { get; internal init; }
        public SisbaseCommandSystem CommandSystem { get; internal init; }
        internal PrefixResolver PrefixResolver { get; set; }

        public SisbaseBot(DiscordSocketClient client, MainConfig config) {
            Client = client;
            Config = config;
            CommandSystem = new(Client);

            PrefixResolver = new RealTimePrefixResolver(CommandSystem, Config.Data.Prefixes.ToArray());
        }

        public SisbaseBot(DiscordSocketClient client, FileInfo configFile) {
            if (configFile.Extension != "json") return;

            Client = client;

            Config = new();
            Config.Create(configFile);
            CommandSystem = new(Client);

            PrefixResolver = new RealTimePrefixResolver(CommandSystem, Config.Data.Prefixes.ToArray());
        }

        public void UsePrefixResolver(PrefixResolver resolver) {
            PrefixResolver = resolver;
        }
    }
}
