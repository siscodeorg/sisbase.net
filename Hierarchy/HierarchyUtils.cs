// unset

using System.Linq;

using Discord;
using Discord.WebSocket;

namespace sisbase.Hierarchy {
    public static class HierarchyUtils {
        public static IRole GetHighestRole(SocketGuildUser user)
            => user.Roles.OrderByDescending(x => x.Position).FirstOrDefault();
       
        public static bool IsAbove(this IRole left, IRole right)
            => left.Position > right.Position;

        public static bool IsAbove(this SocketGuildUser user, IRole role)
            => user.Hierarchy > role.Position;

        public static bool IsAbove(this SocketGuildUser left, SocketGuildUser right)
            => left.Hierarchy > right.Hierarchy;
    }
}