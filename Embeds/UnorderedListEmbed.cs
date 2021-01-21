using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;

using sisbase.Common;

namespace sisbase.Embeds {
    public static partial class EmbedBase {
        public static Embed UnorderedListEmbed<T>(string name,
            List<T> list) {
            EmbedBuilder embed = new();
            var description = list.Count == 0 ? "No Data." : string.Join("\n", list.Select(x => x.ToString()));
            embed.WithFooter($"「sisbase」・ {VersionUtils.GetVersion()}");
            embed.WithAuthor(name);
            embed.WithDescription(description);
            embed.WithColor(Color.LightOrange);
            return embed.Build();
        }
    }
}
