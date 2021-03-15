using System;
using System.Collections.Generic;

using Discord;

namespace sisbase.Streams {
    public static class MessageStreams {
        /// <summary>
        /// Gets the previous N messages from a given <see cref="ITextChannel"/>.
        /// </summary>
        /// <param name="channel">The channel to get the messages from.</param>
        /// <param name="limit">The number of messages.</param>
        /// <returns>A collection of messages.</returns>
        public static async IAsyncEnumerable<IMessage> StreamMessagesAsync (this ITextChannel channel, int limit = 100) {
            await foreach(var group in channel.GetMessagesAsync(limit)) {
                foreach(var elem in group) {
                    yield return elem;
                }
            }
        }
        
        /// <summary>
        /// Gets the previous N messages prior to a given <see cref="IMessage"/>.
        /// </summary>
        /// <param name="channel">The channel to get the messages from.</param>
        /// <param name="from">The starting message to get the messages from.</param>
        /// <param name="limit">The number of messages.</param>
        /// <returns></returns>
        public static async IAsyncEnumerable<IMessage> StreamMessagesAsync (this ITextChannel channel, IMessage from, int limit = 100) {
            await foreach(var group in channel.GetMessagesAsync(limit: limit,fromMessage:from,dir:Direction.Before)) {
                foreach(var elem in group) {
                    yield return elem;
                }
            }
        }
        
        /// <summary>
        /// Gets all the messages from a given <see cref="ITextChannel"/> in descending order (Newest -> Oldest).
        /// <br></br> <br></br>
        /// WARNING : This may take a LOOOOOONG time depending on how many messages there are in a channel if left
        /// without a way to stop. Use this either when you know the IDs or with a predicate.
        /// </summary>
        /// <param name="channel">The channel to get the messages from.</param>
        /// <returns>A Collection of <see cref="IMessage"/>.</returns>
        public static async IAsyncEnumerable<IMessage> StreamAllMessagesAsync(this ITextChannel channel) {
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