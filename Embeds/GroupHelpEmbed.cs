using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using sisbase.CommandsNext;

namespace sisbase.Embeds {
    public static partial class EmbedBase {
        public static async Task<Embed> GroupHelpEmbedAsync(ModuleInfo group, ICommandContext context, IServiceProvider provider) {
            var (name, summary, parent) = (group.Aliases[0], group.Summary, group.Parent);

            var commands = await group.GetExecutableCommandsAsync(context, provider);
            var strings = commands.Select(x => FormatSubcommand(x, name));

            EmbedBuilder embed = new();
            embed.WithColor(Color.Purple);
            embed.WithAuthor($"Group : {name} | Help");
            if (!string.IsNullOrWhiteSpace(summary)) embed.WithDescription(summary);
            if (parent != default) embed.AddField("Parent", parent.Aliases[0]);
            if (commands.Any()) embed.AddField("Sub-Commands", string.Join("\n", strings));
            return embed.Build();
        }

        internal static string FormatSubcommand(CommandInfo command, string parent) {
            var commandName = command.Aliases[0].Split(" ").Last();
            if (commandName == parent) return "";
            StringBuilder str = new();
            str.Append($"**{commandName}**");
            if (!string.IsNullOrWhiteSpace(command.Summary)) str.Append($" - *{command.Summary}*");
            return str.ToString();
        }
        public static Task<Embed> GroupHelpEmbedAsync(ModuleInfo group, ICommandContext context, SisbaseCommandSystem system)
            => GroupHelpEmbedAsync(group, context, system._provider);
    }
}
