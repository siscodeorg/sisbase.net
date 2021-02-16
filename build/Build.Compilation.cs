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

    [GitVersion(Framework = "net5.0")] readonly GitVersion GitVersion;

    string FileVersion;
    string InformationalVersion;
    string AssemblyVersion;

    Target GenerateVersion => _ => _
        .Executes(() => {
            InformationalVersion = GitVersion.InformationalVersion;
            FileVersion = GitVersion.NuGetVersionV2;
            AssemblyVersion = GitVersion.AssemblySemVer;
            
            Logger.Info($"InformationalVersion : {InformationalVersion}");
            Logger.Info($"FileVersion : {FileVersion}");
            Logger.Info($"AssemblyVersion : {AssemblyVersion}");
        });

    Target Compile => _ => _
        .DependsOn(GenerateVersion, Restore)
        .Executes(() => {
            DotNetBuild(_ => _
                .SetProjectFile(Solution.GetProject("sisbase"))

                .SetAssemblyVersion(AssemblyVersion)
                .SetFileVersion(FileVersion)
                .SetInformationalVersion(InformationalVersion)

                .SetConfiguration(Configuration)

                .EnableNoRestore()
            );
        });

    Target CompileBot => _ => _
        .DependsOn(Restore)
        .Executes(() => {
            DotNetBuild(_ => _
                .SetProjectFile(Solution.GetProject("sisbase.TestBot"))

                .SetConfiguration(Configuration)

                .EnableNoRestore()

                .SetOutputDirectory(TestBotPath)
            );
        });
}

