using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sisbase.Systems {
    public abstract class ClientSystem : BaseSystem {
        public BaseSocketClient Client;
        public virtual async Task ApplyToClient(BaseSocketClient client) { }
    }
}
