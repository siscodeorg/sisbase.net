using Discord.WebSocket;

using sisbase.Configuration;

using System;
using System.IO;
using System.Threading.Tasks;

namespace sisbase.TestBot {
    class Program {
        static async Task Main(string[] args) {
            var client = new DiscordSocketClient();
            var bot = new SisbaseBot(client, new FileInfo($"{Directory.GetCurrentDirectory()}/config.json"));
            SystemConfig config = new();
            config.Create(new FileInfo($"{Directory.GetCurrentDirectory()}/systems.json"));
            bot.UseSystemsApi(config);
            await bot.InstallCommandsAsync(typeof(Program).Assembly);
            await bot.InstallSystemsAsync(typeof(Program).Assembly);
            await bot.StartAsync();
        }
    }
}
