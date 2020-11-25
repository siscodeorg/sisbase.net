using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sisbase.CommandsNext {
    public class RealTimePrefixResolver : PrefixResolver {
        private string[] _prefixes;

        public RealTimePrefixResolver(SisbaseCommandSystem commandSystem)
            : base(commandSystem._client) {
            _prefixes = Array.Empty<string>();
        }
        public RealTimePrefixResolver(SisbaseCommandSystem commandSystem, params string[] prefixes)
            : base(commandSystem._client) {
            _prefixes = prefixes;
        }

        public override async Task<int> GetArgumentPositionAsync(SocketUserMessage message) {
            int argPos = 0;

            if (_prefixes == Array.Empty<string>()|| message.Author.IsBot) {
                return argPos;
            }

            if (message.HasMentionPrefix(User, ref argPos) || CheckPrefixes(message, ref argPos)) {
                return argPos;
            }
            return argPos;
        }

        internal bool CheckPrefixes(SocketUserMessage message, ref int argpos) {
            int value = 0;
            bool status = false;
            foreach(var prefix in _prefixes) {
                if (status = message.HasStringPrefix(prefix, ref value)) break;
            }
            argpos = value;
            return status;
        }
    }
}
