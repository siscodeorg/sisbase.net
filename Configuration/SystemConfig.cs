using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sisbase.Configuration {
    public class SystemConfig : IConfiguration {
        [JsonProperty] public Dictionary<string, SystemData> Systems { get; set; }
        public string Path { get; set; }

        public void Create(FileInfo file) {
            Path = file.FullName;
            if(file.Exists) {
                var data = JsonConvert.DeserializeObject<SystemConfig>(File.ReadAllText(Path));
                Systems = data.Systems;
                return;
            }
            Update();
        }

        public void Update() => File.WriteAllText(Path, JsonConvert.SerializeObject(this, Formatting.Indented));
    }

    public record SystemData (string Name, bool Enabled);
}
