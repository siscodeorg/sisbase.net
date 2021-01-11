using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;

namespace sisbase.TestBot {
    class Program {
        static async Task Main(string[] args) {
            var client = new DiscordSocketClient();
            var bot = new SisbaseBot(client, new FileInfo($"{Directory.GetCurrentDirectory()}/config.json"));
            await bot.InstallCommandsAsync(typeof(Program).Assembly);
            await bot.StartAsync();
        }
    }
}
