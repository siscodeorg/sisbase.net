using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;

using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

partial class Build : NukeBuild {
    [PathExecutable] readonly Tool Git;

    const string DevelopBranch = "beta";
    const string AlphaBranch = "alpha";
    const string ReleaseBranch = "release";

    const string FeaturePrefix = "feature";

    bool CheckForBranch(string Name) {
        bool result = false;
        try {
            Git($"rev-parse --verify --silent refs/heads/{Name}");
            result = true;
        } catch {
            result = false;
        }
        return result;
    }
}

