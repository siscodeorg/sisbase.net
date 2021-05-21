using System;
using System.Linq;

using Discord;

namespace sisbase.Embeds {
    public static class ExtensionEmbeds {

        public static EmbedFieldBuilder ToEmbedFieldBuilder(this EmbedField field) 
            => new() {Name = field.Name, Value = field.Value, IsInline = field.Inline};

        public static EmbedFooterBuilder ToEmbedFooterBuilder(this EmbedFooter footer) 
            => new() {IconUrl = footer.IconUrl, Text =  footer.Text};
    }
}
