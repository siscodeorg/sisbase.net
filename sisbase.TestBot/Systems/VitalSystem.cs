// unset

using System.Threading.Tasks;

using sisbase.Systems;
using sisbase.Systems.Attributes;

namespace sisbase.TestBot.Systems {
    [Vital]
    public class VitalSystem : ClientSystem {
        public override async Task Activate() {
            Name = "Vital";
            Description = "System to test the functionality of vital systems";
        }

        public override async Task Deactivate() {
            
        }
    }
}