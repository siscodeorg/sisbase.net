// unset

using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Nuke.Common;
using Nuke.Common.IO;

namespace SisbaseBuildTools {
    public static class SisbasePackageResolver {
        private const int DefaultTimeout = 2000;
        
        public static async Task<string> GetLatestGithubPackageVersion(string repoOwner,string packageId,string actor, string token , int? timeout = null)
        {
            try {
                var url = $"https://nuget.pkg.github.com/{repoOwner}/{packageId.ToLowerInvariant()}/index.json";
                    
                var jsonString = await HttpTasks.HttpDownloadStringAsync(url,
                    requestConfigurator: x => x.Timeout = timeout ?? DefaultTimeout,
                    clientConfigurator: y => { 
                        y.Credentials = new NetworkCredential(actor,token);
                        return y;
                    }
                );
                
                var jsonObject = JsonConvert.DeserializeObject<JObject>(jsonString);
                var jToken = jsonObject["items"]?.First()["items"]?.First()["catalogEntry"];
                return jToken != null ? jToken["version"]?.ToString() : "";
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }
    }
}