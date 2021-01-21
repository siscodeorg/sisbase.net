using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord.Commands;

using sisbase.CommandsNext;
using sisbase.Embeds;

namespace sisbase.TestBot {
    [Group("embedbase")]
    public class EmbedBaseUsage : ModuleBase<SisbaseCommandContext> {
        [Command("inputEmbed")]
        public async Task inputEmbed()
            => await ReplyAsync(embed: EmbedBase.InputEmbed("Actually, nothing. This is just a test!"));

        [Command("outputEmbed")]
        public async Task outputEmbed()
            => await ReplyAsync(embed: EmbedBase.OutputEmbed("Yet Another Test."));

        [Command("orderedListEmbed")]
        public async Task orderedListEmbed() {
            List<string> list = new();
            list.Add("This is a value");
            list.Add("This is a another value");

            await ReplyAsync(embed: EmbedBase.OrderedListEmbed("Random List", list));
        }

        [Command("unorderedListEmbed")]
        public async Task unorderedListEmbed() {
            List<string> list = new();
            list.Add("This is a value");
            list.Add("This is a another value");

            await ReplyAsync(embed: EmbedBase.UnorderedListEmbed("Random List", list));
        }
    }
}
