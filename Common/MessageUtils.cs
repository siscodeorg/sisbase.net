using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Discord;

using sisbase.Unicode;

namespace sisbase.Common {
    public static class MessageUtils {
        public static List<int> GetNumbers(this IMessage message) {
            Regex regex = new(@"\d+");
            var matches = regex.Matches(message.Content);
            return !matches.Any() ? new List<int>() : matches.Select(x => int.Parse(x.Value)).ToList();
        }

        public static List<Emote> GetEmotes(this IMessage message) {
            Regex regex = new(@"\<\:[^><\s]+\:\d+\>");
            var matches = regex.Matches(message.Content);
            return !matches.Any() ? new List<Emote>() : matches.Select(x => Emote.Parse(x.Value)).ToList();
        }

        public static List<Emoji> GetEmojis(this IMessage message) {
            Regex regex = new(@"[^\p{P}\p{S}\w\s]+");
            var matches = regex.Matches(message.Content);
            foreach (Match match in matches) {
                GetAllEmoji(match.Value);
            }

            return !matches.Any()
                ? new List<Emoji>()
                : matches
                    .SelectMany(x => GetAllEmoji(x.Value))
                    .Select(x => new Emoji(x.PrimaryNameWithColons))
                    .ToList();
        }

        internal static List<EmojiInfo> GetAllEmoji(string utf32Sequence) {
            List<EmojiInfo> save = new();
            if (string.IsNullOrWhiteSpace(utf32Sequence)) return save;
            var accumulator = "";
            foreach (var c in utf32Sequence) {
                accumulator += c;
                if (Emojis.FromSurrogates.ContainsKey(accumulator)) {
                    save.Add(Emojis.FromSurrogates[accumulator]); 
                }
            }

            save = new List<EmojiInfo> {save.Last()};
            var newerString = utf32Sequence.Replace(save.Last().Surrogates, "");
            save.AddRange(GetAllEmoji(newerString));
            return save;
        }
    }
}