// unset

using System;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using sisbase.CommandsNext;
using sisbase.Streams;

namespace sisbase.TestBot.Streams {
    public class MessageStreamTest : ModuleBase<SisbaseCommandContext> {
        int count;
        
        [Command("mstream")]
        public async Task mstream() {
            count = 0;
            var ch = Context.Channel as SocketTextChannel;
            var edit = await ReplyAsync("Waiting...");
            IMessage msg = null;
            
            await foreach (var message in ch.StreamAllMessagesAsync()) {
                msg = message;
                count += 1;
            }

            await ReplyAsync(embed: getMessageDetails(msg));
        }

        private Embed getMessageDetails(IMessage message) {
            EmbedBuilder builder = new();
            
            builder
                .WithDescription($"Author : {message.Author.Mention}\n" +
                                 $"Content : {message.Content}\n" +
                                 $"Link : {message.GetJumpUrl()}")
                .WithFooter($"Timestamp : {message.Timestamp} | Count : {count}")
                .WithAuthor("Testing MessageStream");

            return builder.Build();
        }
    }
}