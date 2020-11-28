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
        /// Reason for ugly : Feature request via PR on hold.<br></br>
        /// Dies if Discord.Net#1700 Gets merged.
        /// </summary>
        internal static async Task<(IResult, Optional<CommandMatch>)> ValidateAndGetBestMatch(SearchResult matches, ICommandContext context, IServiceProvider provider, MultiMatchHandling multiMatchHandling = MultiMatchHandling.Exception) {
            if (!matches.IsSuccess)
                return (matches, Optional.Create<CommandMatch>());

            var commands = matches.Commands;
            var preconditionResults = new Dictionary<CommandMatch, PreconditionResult>();

            foreach (var command in commands) {
                preconditionResults[command] = await command.CheckPreconditionsAsync(context, provider);
            }

            var successfulPreconditions = preconditionResults
                .Where(x => x.Value.IsSuccess)
                .ToArray();

            if (successfulPreconditions.Length == 0) {
                var bestCandidate = preconditionResults
                   .OrderByDescending(x => x.Key.Command.Priority)
                   .FirstOrDefault(x => !x.Value.IsSuccess);

                return (bestCandidate.Value, bestCandidate.Key);
            }

            var parseResults = new Dictionary<CommandMatch, ParseResult>();

            foreach (var pair in successfulPreconditions) {
                var parseResult = await pair.Key.ParseAsync(context, matches, pair.Value, provider).ConfigureAwait(false);

                if (parseResult.Error == CommandError.MultipleMatches) {
                    IReadOnlyList<TypeReaderValue> argList, paramList;
                    switch (multiMatchHandling) {
                        case MultiMatchHandling.Best:
                            argList = parseResult.ArgValues.Select(x => x.Values.OrderByDescending(y => y.Score).First()).ToImmutableArray();
                            paramList = parseResult.ParamValues.Select(x => x.Values.OrderByDescending(y => y.Score).First()).ToImmutableArray();
                            parseResult = ParseResult.FromSuccess(argList, paramList);
                            break;
                    }
                }

                parseResults[pair.Key] = parseResult;
            }

            var weightedParseResults = parseResults
               .OrderByDescending(x => CalculateScore(x.Key, x.Value));

            var successfulParses = weightedParseResults
                .Where(x => x.Value.IsSuccess)
                .ToArray();

            if (successfulParses.Length == 0) {
                var bestMatch = parseResults
                    .FirstOrDefault(x => !x.Value.IsSuccess);

                return (bestMatch.Value, bestMatch.Key);
            }

            var chosenOverload = successfulParses[0];

            return (chosenOverload.Value, chosenOverload.Key);
        }


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
