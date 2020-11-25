using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace sisbase.CommandsNext {
    public class SisbaseCommandSystem {
        internal PrefixResolver _prefixResolver;
        internal DiscordSocketClient _client;
        internal CommandService _commandService = new();
        internal IServiceProvider _provider;
        internal ServiceCollection _collection = new();

        public SisbaseCommandSystem(DiscordSocketClient client) {
            _client = client;
            _prefixResolver = new RealTimePrefixResolver(this);
            _provider = InitialServiceCollection.BuildServiceProvider();
        }

        public SisbaseCommandSystem(DiscordSocketClient client, SisbaseCommandSystemConfiguration config) {
            _client = client;
            _prefixResolver = config.PrefixResolver ?? new RealTimePrefixResolver(this);
            config.Services?.Invoke(InitialServiceCollection);
            _provider = InitialServiceCollection.BuildServiceProvider();
        }

        public async Task InstallCommandsAsync(Assembly assembly) {
            _client.MessageReceived += GenericPrefixResolver;
            await _commandService.AddModulesAsync(assembly, _provider);
        }

        private async Task GenericPrefixResolver(SocketMessage message) {
            if (message is not SocketUserMessage msg) return;
            var argPos = await _prefixResolver.GetArgumentPositionAsync(msg);
            if (argPos == 0) return;
            var ctx = new SocketCommandContext(_client, msg);
            await _commandService.ExecuteAsync(
                    context: ctx,
                    argPos: argPos,
                    services: _provider
            );
        }

        internal IServiceCollection InitialServiceCollection => _collection
            .AddSingleton(_client)
            .AddSingleton(_commandService);
    }
}
