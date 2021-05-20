using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sisbase.Common;

namespace sisbase.Logging {
    public static class Logger {

        internal static Dictionary<LogLevel, ConsoleColor> _colorMap = new(){
            [LogLevel.LOG] = ConsoleColor.Cyan,
            [LogLevel.WARN] = ConsoleColor.Yellow,
            [LogLevel.ERROR] = ConsoleColor.Red,
        };

        public static void Log(string message) => GenericLog("", message, LogLevel.LOG);
        public static void Log(string source,
            string message) => GenericLog(source, message, LogLevel.LOG);
        public static void Warn(string message) => GenericLog("", message, LogLevel.WARN);
        public static void Warn(string source,
            string message) => GenericLog(source, message, LogLevel.WARN);
        public static void Error(string message) => GenericLog("", message, LogLevel.ERROR);
        public static void Error(string source,
            string message) => GenericLog(source, message, LogLevel.ERROR);
	
	public static void Error(this SisbaseResult error, string source) => Error(error.Error, source);


        internal static void GenericLog(string source, string message, LogLevel level) {
            Console.ForegroundColor = _colorMap[level];
            Console.Write($"{level,-5} | {source} [");
            Console.ResetColor();
            Console.Write($"{ DateTime.Now:u}");
            Console.ForegroundColor = _colorMap[level];
            Console.Write($"] {message}\n");
            Console.ResetColor();
        }
    }
}
