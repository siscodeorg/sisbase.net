using System;
using System.Linq;

using Discord;

namespace sisbase.Embeds {
    public static class ExtensionEmbeds {
    
        public static Embed Mutate(this Embed embed, Action<EmbedBuilder> func) {
            var eb = embed.ToEmbedBuilder();
            func(eb);
            return eb.Build();
        }

        public static EmbedFieldBuilder ToEmbedFieldBuilder(this EmbedField field) 
            => new() {Name = field.Name, Value = field.Value, IsInline = field.Inline};

        public static EmbedFooterBuilder ToEmbedFooterBuilder(this EmbedFooter footer) 
            => new() {IconUrl = footer.IconUrl, Text =  footer.Text};

        public static EmbedBuilder ToEmbedBuilder(this Embed embed) {
            EmbedBuilder dnetBuilder = new();
            var dnetFields = embed.Fields.Select(x => x.ToEmbedFieldBuilder());
            dnetBuilder = dnetBuilder
                .WithTitle(embed.Title)
                .WithDescription(embed.Description)
                .WithUrl(embed.Url)
                .WithFields(dnetFields);

            if (embed.Color.HasValue) dnetBuilder = dnetBuilder.WithColor(embed.Color.Value);
            if (embed.Timestamp.HasValue) dnetBuilder = dnetBuilder.WithTimestamp(embed.Timestamp.Value);
            if (embed.Footer.HasValue) dnetBuilder = dnetBuilder.WithFooter(embed.Footer.Value.ToEmbedFooterBuilder());
            if (embed.Thumbnail.HasValue) dnetBuilder = dnetBuilder.WithThumbnailUrl(embed.Thumbnail.Value.Url);

            return dnetBuilder;
        }
    }
}
