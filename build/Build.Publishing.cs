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
    Target Pack => _ => _
        .DependsOn(Clean, Compile)
        .Produces(ArtifactsPath / "*.nupkg")
        .Executes(() => {
            DotNetPack(_ => _
                .SetProject(Solution.GetProject("sisbase"))

                .SetConfiguration(Configuration.Release)

                .SetAssemblyVersion(FileVersion)
                .SetFileVersion(FileVersion)
                .SetInformationalVersion(InformationalVersion)

                .SetOutputDirectory(ArtifactsPath)
            );
        });
}
