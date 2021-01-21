using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;

using sisbase.Common;

namespace sisbase.Embeds {
    public static partial class EmbedBase {
        public static Embed InputEmbed(string query) {
            EmbedBuilder embed = new();
            embed.WithFooter($"「sisbase」・ {VersionUtils.GetVersion()}");
            embed.WithDescription($"Please Type: {query}");
            embed.WithColor(Color.Teal);
            return embed.Build();
        }
    }
}
