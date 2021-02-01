using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using sisbase.CommandsNext;
using sisbase.Common;

namespace sisbase.Embeds {
    public static partial class EmbedBase {
        public static async Task<Embed> HelpEmbedAsync(CommandService commandService, ICommandContext context, IServiceProvider provider) {
            var commands = await commandService.GetExecutableCommandsAsync(context, provider);
            var modules = commands.Select(x => x.Module).Distinct();
            var toplevel = commands.Where(x => x.Module.Aliases[0] == "");
            var formattedTopLevel = toplevel.Select(x => $"`{x.Aliases[0]}`").Distinct();

            EmbedBuilder embed = new();
            embed.WithFooter($"「sisbase」・ {VersionUtils.GetVersion()}");
            embed.WithColor(Color.DarkMagenta);
            embed.WithAuthor("Showing all available commands | Help");

            if (modules.Any()) {
                foreach(var module in modules) {
                    if (module.Aliases[0] == "") continue;
                    var desc = string.IsNullOrWhiteSpace(module.Summary) ? "No description found." : module.Summary;
                    embed.AddField($"{module.Aliases[0]}", desc);
                }
            }

            if(toplevel.Any()) embed.AddField("❓ Miscelaneous", string.Join(" ", formattedTopLevel));
            return embed.Build();
        }

        public static Task<Embed> HelpEmbedAsync(SisbaseCommandSystem commandSystem, ICommandContext context)
            => HelpEmbedAsync(commandSystem._commandService, context, commandSystem._provider);
    }
}
