using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord.WebSocket;

using sisbase.Systems;

namespace sisbase.TestBot.Systems {
    public class Client : ClientSystem {
        public override async Task Activate() {
            Name = "Client";
            Description = "Example ClientSystem";
            Client.MessageReceived += Received;
        }

        public override async Task Deactivate() {
            Client.MessageReceived -= Received;
        }

        private async Task Received(SocketMessage arg) {
            if (arg.Content.ToLowerInvariant() == "ping") {
                await arg.Channel.SendMessageAsync($"{Client.Latency} **ms**");
            }
        }
    }
}
