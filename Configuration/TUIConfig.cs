using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sisbase.Configuration {
    internal static class TUIConfig {
        internal static MainConfigData GetConfig() {
            Console.Clear();
            Console.WriteLine("Please type the bot token\n");
            Console.ForegroundColor = ConsoleColor.Red;
            string token = Console.ReadLine();
            return new(token);
        }
    }
}
