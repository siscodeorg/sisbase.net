// unset

using System.Linq;

using Discord;
using Discord.WebSocket;

namespace sisbase.Hierarchy {
    public static class HierarchyUtils {
        public static IRole GetHighestRole(SocketGuildUser user)
            => user.Roles.OrderByDescending(x => x.Position).FirstOrDefault();

        
    }
}