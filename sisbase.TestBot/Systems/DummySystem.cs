using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord.WebSocket;

using sisbase.Systems;
using sisbase.Systems.Attributes;
using sisbase.TestBot.Services;

/*
 * System to show the intended utilization of UsesAttribute.
 */

namespace sisbase.TestBot.Systems {
    [Uses(typeof(DummyService))]
    public class DummySystem : ClientSystem {
        public DummyService service { get; set; }
        public override async Task Activate() {
            Client.MessageReceived += MessageCreated;
        }

        public override async Task Deactivate() {
            Client.MessageReceived -= MessageCreated;
        }

        private async Task MessageCreated(SocketMessage arg) {
            if(arg.Content.ToLowerInvariant() == "dummy") {
                await arg.Channel.SendMessageAsync($"DummyService.GetDummyValue() = {service.GetDummyValue()}");
            }
        }
    }
}
