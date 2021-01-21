using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using sisbase.Systems;
using sisbase.Systems.Expansions;

namespace sisbase.TestBot.Systems {
    public class Timed : ClientSystem, Sheduler {
        public TimeSpan Timeout => TimeSpan.FromSeconds(10);

        public Action RunContinuously => async () => {
            Console.WriteLine("heya!");
            await Client.SetGameAsync($"Time is : {DateTime.Now}");
        };

        public override async Task Activate() {
            Name = "Timed";
            Description = "Sample System to test the scheduler expansion";
        }
        public override async Task Deactivate() {

        }
    }
}
