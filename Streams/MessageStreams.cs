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

        public static async IAsyncEnumerable<IMessage> StreamAllMessagesAsync(this SocketTextChannel channel) {
            IMessage last = null;
            var isLastBatch = false;
            int count;

            await foreach (var message in channel.StreamMessagesAsync()) {
                last = message;
                Console.WriteLine($"Last set to {last.Id}");
                yield return message;
            }

            while (!isLastBatch) {
                count = 0;
                IAsyncEnumerable<IMessage> enumerable;
                await foreach (var message in enumerable = channel.StreamMessagesAsync(last)) {
                    last = message;
                    count++;
                    yield return message;
                }

                if (count != 100) isLastBatch = true;
            }
        }
    }
}