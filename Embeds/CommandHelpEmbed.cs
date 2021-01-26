using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using sisbase.Common;

namespace sisbase.Embeds {
    public static partial class EmbedBase {
        public static Embed CommandHelpEmbed(CommandInfo command) {
            EmbedBuilder embed = new();

            var overloads = command.Module.Commands.Where(x => x.Aliases[0] == command.Aliases[0]).ToList();
            var usages = overloads.Select(x => GetUsage(x)).ToList();
            var parameterDetails = overloads.SelectMany(x => x.Parameters).Distinct().Select(y => FormatParameterDetails(y)).ToList();

            string parameters = parameterDetails.Count == 0 ? "No Parameters." : string.Join("\n", parameterDetails);

            embed.WithFooter($"「sisbase」・ {VersionUtils.GetVersion()}");
            embed.WithDescription($"Usage : {string.Join(", ", usages)}");
            embed.AddField("Parameters", parameters);
            embed.WithColor(Color.DarkGreen);
            embed.WithAuthor($"Command : {GetCommandName(command.Aliases[0])} | Help");

            return embed.Build();
        }

        internal static string GetUsage(CommandInfo command) {
            if (!command.Parameters.Any()) {
                return $"`{GetCommandName(command.Aliases[0])}`";
            }
            var parameters = command.Parameters.Select(x => FormatParameter(x));
            return $"`{GetCommandName(command.Aliases[0])} {string.Join(" ", parameters)}`";
        }

        internal static string FormatParameterDetails(ParameterInfo parameter) {
            StringBuilder sb = new($"({parameter.Type.Name}) ");
            if (parameter.IsOptional)
                sb.Append("<Optional> ");
            sb.Append($"{parameter.Name} ");
            if (!string.IsNullOrWhiteSpace(parameter.Summary))
                sb.Append($"- {parameter.Summary}");

            return sb.ToString();
        }

        internal static string FormatParameter(ParameterInfo parameter) {
            if (parameter.IsOptional) {
                return $"<{parameter.Name}>";
            } else {
                return $"[{parameter.Name}]";
            }
        }

        internal static string GetCommandName(string fullName) => fullName.Split(" ").Last();

    }
}
