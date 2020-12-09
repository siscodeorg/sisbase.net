using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sisbase.CommandsNext {
    public class SisbaseCommandSystemConfiguration {
        public PrefixResolver PrefixResolver { get; init; }
        public Action<IServiceCollection> Services { get; init; }
    }
}
