using System;
using System.Collections.Generic;

namespace sisbase.Unicode {
    public class EmojiInfo {
        public string PrimaryName { get; set; }
        public string PrimaryNameWithColons { get; set; }
        public List<string> Names { get; set; }
        public List<string> NamesWithColons { get; set; }
        public string Surrogates { get; set; }
        public List<int> Utf32Codepoints { get; set; }
        public string AssetFileName { get; set; }
        public Uri AssetUrl { get; set; }
    }
}