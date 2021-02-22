using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord.Commands;

using sisbase.Systems;
using sisbase.Systems.Enums;

namespace sisbase.CommandsNext.Preconditions {
    public class UsesAttribute : PreconditionAttribute {
        internal List<Type> systems = new();
        public UsesAttribute(params Type[] types) {
            foreach(var type in types) {
                var response = SystemManager.IsValidType(type);
                if (response.Any(x => !x.IsSucess)) throw new ArgumentException($"{type.Name} isn't a valid system. \n" +
                    $"Reasons :\n" +
                    $"{string.Join(",", response.Select(x => x.Error))}", type.Name);
                systems.Add(type);
            }
        }
        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services) {
            var manager = (SystemManager) services.GetService(typeof(SystemManager));

            if (manager == null) throw new InvalidOperationException("Current ServiceProvider doesn't provide a SystemManager service.\n" +
                "Have you forgotten to add the singleton?");

            var statuses = systems.Select(x => manager.Get(x));
            var nonLoaded = statuses.Where(x => x.Item2 != SystemStatus.LOADED);

            if (nonLoaded.Any())
                return PreconditionResult.FromError("The following dependencies weren't loaded.\n" +
                    $"{string.Join("\n", nonLoaded.Select(x => $"{x.Item1.GetSisbaseTypeName()} - {x.Item2}"))}");

            return PreconditionResult.FromSuccess();
        }
    }
}
