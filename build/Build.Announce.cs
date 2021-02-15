using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Data.Git;
using Data.Git.Enums;

using Discord;
using Discord.Webhook;

using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities;
using Nuke.Common.Utilities.Collections;

using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

partial class Build : NukeBuild {

    Dictionary<string, List<string>> Categorize(IEnumerable<BranchInfo> branches) {
        Dictionary<string, List<string>> result = new();
        foreach (var branch in branches) {
            if (result.ContainsKey(branch.Tag)) {
                result[branch.Tag].Add(branch.Name);
            } else {
                result.Add(branch.Tag, new());
                result[branch.Tag].Add(branch.Name);
            }
        }
        return result;
    }
}

