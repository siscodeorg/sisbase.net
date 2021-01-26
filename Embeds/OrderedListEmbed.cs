using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;

using sisbase.Common;
using sisbase.Embeds.Enums;

namespace sisbase.Embeds {
    public static partial class EmbedBase {
        public static Embed OrderedListEmbed<T>(string name,
            List<T> list,
            CountingBehaviour counting = CountingBehaviour.Default) {
            EmbedBuilder embed = new();
            var description = list.Count == 0 ? "No Data." : string.Join("\n", list.Select(x => formatItem(list.IndexOf(x), x.ToString(), counting)));
            embed.WithFooter($"「sisbase」・ {VersionUtils.GetVersion()}");
            embed.WithAuthor(name);
            embed.WithDescription(description);
            embed.WithColor(Color.LightOrange);
            return embed.Build();
        }

        internal static string formatItem(int position, string value, CountingBehaviour behaviour) {
            return behaviour switch {
                CountingBehaviour.Default => $"{position} - {value}",
                CountingBehaviour.Ordinal => $"{position + 1} - {value}"
            };
        }
    }
}
