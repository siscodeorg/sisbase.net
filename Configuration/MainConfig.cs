using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sisbase.Configuration {
    public class MainConfig : IConfiguration {
        public string Path { get; set; }
        public int Version = 1;
        public MainConfigData Data { get; set; }

        public void Create(FileInfo file) {
            Path = file.FullName;

            if (!file.Exists) {
                Data = TUIConfig.GetConfig();
            } else {
                Data = JsonConvert.DeserializeObject<MainConfig>(File.ReadAllText(Path)).Data;
            }
            Update();
        }
        public void Update() => File.WriteAllText(Path, JsonConvert.SerializeObject(this));

    }

    public record MainConfigData {
        [JsonProperty] internal string Token { get; init; }
        [JsonProperty] public ulong MasterId { get; set; }
        [JsonProperty] public List<ulong> PuppetId { get; set; }
        [JsonProperty] public List<string> Prefixes { get; set; }
        [JsonProperty] internal Dictionary<string, object> CustomSettings { get; set; }
        public MainConfigData(string token) => Token = token;
    }
}
