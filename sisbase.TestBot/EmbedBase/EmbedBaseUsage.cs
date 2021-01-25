using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Discord.Commands;

using sisbase.CommandsNext;
using sisbase.Embeds;

namespace sisbase.TestBot {
    [Group("embedbase")]
    public class EmbedBaseUsage : ModuleBase<SisbaseCommandContext> {
        public SisbaseCommandSystem system { get; set; }
        [Command("")]
        public async Task GroupHelpEmbed()
            => await ReplyAsync(embed: await EmbedBase.GroupHelpEmbedAsync(Context.Command.Module, Context, system));

        [Command("inputEmbed"), Summary("Displays an example InputEmbed")]
        public async Task inputEmbed()
            => await ReplyAsync(embed: EmbedBase.InputEmbed("Actually, nothing. This is just a test!"));

        [Command("outputEmbed"), Summary("Displays an example OutputEmbed")]
        public async Task outputEmbed()
            => await ReplyAsync(embed: EmbedBase.OutputEmbed("Yet Another Test."));

        [Command("orderedListEmbed"), Summary("Displays an example OrderedListEmbed")]
        public async Task orderedListEmbed() {
            List<string> list = new();
            list.Add("This is a value");
            list.Add("This is a another value");

            await ReplyAsync(embed: EmbedBase.OrderedListEmbed("Random List", list));
        }

        [Command("unorderedListEmbed"), Summary("Displays an example UnorderedListEmbed")]
        public async Task unorderedListEmbed() {
            List<string> list = new();
            list.Add("This is a value");
            list.Add("This is a another value");

            await ReplyAsync(embed: EmbedBase.UnorderedListEmbed("Random List", list));
        }

        [Command("commandHelpEmbed"), Summary("Displays an example CommandHelpEmbed")]
        public async Task commandHelpEmbed() {
            await ReplyAsync(embed: EmbedBase.CommandHelpEmbed(command: Context.Command));
        }

        [Command("commandHelpEmbed"), Summary("Displays an example CommandHelpEmbed2")]
        public async Task commandHelpEmbed([Summary("This thing has an int and its called yay!")] int yay) {
            Random r = new();
            await ReplyAsync($"yay! you called the useful command. number is {r.Next(0-yay, yay)}.");
        }
    }
}
