using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Discord;

namespace sisbase.Common {
    public static class MessageUtils {
        public static List<int> GetNumbers(this IMessage message) {
            Regex regex = new(@"\d+");
            var matches = regex.Matches(message.Content);
            return !matches.Any() ? new List<int>() : matches.Select(x => int.Parse(x.Value)).ToList();
        }
    }
}