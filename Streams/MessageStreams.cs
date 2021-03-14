using System;
using System.Collections.Generic;

using Discord;
using Discord.WebSocket;

namespace sisbase.Streams {
    public static class MessageStreams {
        public static async IAsyncEnumerable<IMessage> StreamMessagesAsync (this SocketTextChannel channel) {
            await foreach(var group in channel.GetMessagesAsync()) {
                foreach(var elem in group) {
                    yield return elem;
                }
            }
        }
        
        public static async IAsyncEnumerable<IMessage> StreamMessagesAsync (this SocketTextChannel channel, IMessage from) {
            await foreach(var group in channel.GetMessagesAsync(fromMessage:from,dir:Direction.Before)) {
                foreach(var elem in group) {
                    yield return elem;
                }
            }
        }
    }
}