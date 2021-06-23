using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using sisbase.CommandsNext.Extensions;
using sisbase.Workarounds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Discord;

using sisbase.Logging;

namespace sisbase.CommandsNext {
    public class SisbaseCommandSystem {
        internal PrefixResolver _prefixResolver;
        internal BaseSocketClient _client;
        internal CommandService _commandService = new();
        internal IServiceProvider _provider;
        internal ServiceCollection _collection = new();

        public SisbaseCommandSystem(BaseSocketClient client) {
            _client = client;
            _prefixResolver = new RealTimePrefixResolver(this);
            _provider = initialServiceCollection.BuildServiceProvider();
        }

        public SisbaseCommandSystem(BaseSocketClient client, SisbaseCommandSystemConfiguration config) {
            _client = client;
            _prefixResolver = config.PrefixResolver ?? new RealTimePrefixResolver(this);
            config.Services?.Invoke(initialServiceCollection);
            _provider = initialServiceCollection.BuildServiceProvider();
        }

        public async Task InstallCommandsAsync(Assembly assembly) {
            _client.MessageReceived += GenericPrefixResolver;
            await _commandService.AddModulesAsync(assembly, _provider);
        }

        private async Task GenericPrefixResolver(SocketMessage message) {
            if (message is not SocketUserMessage msg) return;
            var argPos = await _prefixResolver.GetArgumentPositionAsync(msg);
            if (argPos == 0) return;
           
            ICommandContext ctx;
            switch (_client) {
                case DiscordShardedClient _sharded:
                    ctx = new ShardedCommandContext(_sharded, msg);
                    break;
                case DiscordSocketClient _socket:
                    ctx = new SocketCommandContext(_socket, msg);
                    break;
                default:
                    Logger.Error($"Attempted using unmapped {_client.GetType()} on SisbaseCommandSystem. Please open an issue on sisbase.net's repository.");
                    return;
            }

            var commands = _commandService.Search(message.Content[argPos..]);
            var (response, command) = await Ugly.ValidateAndGetBestMatch(commands, ctx, _provider);
            if (!response.IsSuccess) return;
            if (response is not ParseResult parse) return;
            var sctx = ctx.AsSisbaseContext(command.Value.Command);
            await command.Value.ExecuteAsync(sctx, parse, _provider);
        }

        internal IServiceCollection initialServiceCollection => _collection
            .AddSingleton(_client)
            .AddSingleton(this)
            .AddSingleton(_commandService);
    }
}
