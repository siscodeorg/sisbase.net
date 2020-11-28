using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sisbase.Workarounds {
    // Welcome to the uglies.
    // You shouldn't even be seeing this unless you are code-digging or working on the lib directly.
    // All the functions stated below are mere internal workarounds that without knowing the inner
    // workings could break stuff fabulously.
    // Only use this if you are aware of what it does and why its here.
    internal static partial class Ugly {

        /// <summary>
        /// Reason for Ugly : Clone of library code and dependency of ValidateAndGetBestMatch. <br></br>
        /// Dies if Discord.Net#1700 Gets merged.
        /// </summary>
        private static float CalculateScore(CommandMatch match, ParseResult parseResult) {
            float argValuesScore = 0, paramValuesScore = 0;

            if (match.Command.Parameters.Count > 0) {
                var argValuesSum = parseResult.ArgValues?.Sum(x => x.Values.OrderByDescending(y => y.Score).FirstOrDefault().Score) ?? 0;
                var paramValuesSum = parseResult.ParamValues?.Sum(x => x.Values.OrderByDescending(y => y.Score).FirstOrDefault().Score) ?? 0;

                argValuesScore = argValuesSum / match.Command.Parameters.Count;
                paramValuesScore = paramValuesSum / match.Command.Parameters.Count;
            }

            var totalArgsScore = (argValuesScore + paramValuesScore) / 2;
            return match.Command.Priority + totalArgsScore * 0.99f;
        }
    }
}
