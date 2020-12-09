using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sisbase.CommandsNext {
    public class SisbaseCommandContext : ICommandContext {

        /// <inheritdoc/>
        public IDiscordClient Client { get; }
        /// <inheritdoc/>
        public IGuild Guild { get; }
        /// <inheritdoc/>
        public IMessageChannel Channel { get; }
        /// <inheritdoc/>
        public IUser User { get; }
        /// <inheritdoc/>
        public IUserMessage Message { get; }
        /// <summary>
        /// The command being called
        /// </summary>
        public CommandInfo Command { get; }

        public SisbaseCommandContext
           (IDiscordClient client,
            IUserMessage message,
            CommandInfo command) {
            Client = client;
            Guild = (message.Channel as IGuildChannel)?.Guild;
            Channel = message.Channel;
            User = message.Author;
            Message = message;
            Command = command;
        }
    }
}
