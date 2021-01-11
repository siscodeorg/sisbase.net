using Discord.WebSocket;
using sisbase.CommandsNext;
using sisbase.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sisbase {
    public class SisbaseBot {
        public DiscordSocketClient Client { get; internal init;}
        public MainConfig Config { get; internal init; }
        public SisbaseCommandSystem CommandSystem { get; internal init; }
        internal PrefixResolver PrefixResolver { get; init; }
    }
}
