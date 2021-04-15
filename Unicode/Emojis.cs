using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;

using Newtonsoft.Json;

using sisbase.Common;
using sisbase.Logging;

namespace sisbase.Unicode {
    public static class Emojis {
        
        /// <summary>
        /// Provides a map [discord name (:ok_hand:) -> EmojiInfo]
        /// </summary>
        public static Dictionary<string, EmojiInfo> FromDiscordNames;
        
        /// /// <summary>
        /// Provides a map [surrogate (👌) -> EmojiInfo]
        /// </summary>
        public static Dictionary<string, EmojiInfo> FromSurrogates;
        

        static Emojis() {
            try {
                var str =  ReflectionUtils.GetResource("sisbase.Resources.emojis.json");
                using StreamReader reader = new(str);
                var file = JsonConvert.DeserializeObject<EmojiFile>(reader.ReadToEnd());
                FromDiscordNames = file.emojiDefinitions.SelectMany(x => {
                        var pairs = x.NamesWithColons.Select(y => new KeyValuePair<string, EmojiInfo>(y, x));
                        return pairs;
                    }
                ).ToDictionary(x => x.Key, x => x.Value);

                FromSurrogates = file.emojiDefinitions.Select(x => new KeyValuePair<string, EmojiInfo>(x.Surrogates, x)
                ).ToDictionary(x => x.Key, x => x.Value);
                
            } catch (Exception e) {
                Logger.Error(e.ToString());
            }
        }
    }
}