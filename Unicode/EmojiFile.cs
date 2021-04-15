// unset

using System;
using System.Collections.Generic;

namespace sisbase.Unicode {
    public class EmojiFile {
        public string[] donate { get; set; }
        public string version { get; set; }
        public DateTime versionTimestamp { get; set; }
        public List<EmojiInfo> emojiDefinitions { get; set; }
    }
}