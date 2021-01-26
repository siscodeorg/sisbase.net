using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;

using sisbase.Common;

namespace sisbase.Embeds {
    public static partial class EmbedBase {
        public static Embed OutputEmbed(string message) {
            EmbedBuilder embed = new();
            embed.WithFooter($"「sisbase」・ {VersionUtils.GetVersion()}");
            embed.WithDescription($"{message}");
            embed.WithColor(Color.Green);
            return embed.Build();
        }
    }
}
