using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
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

    [CI] readonly GitHubActions GithubActions;

    [Parameter] readonly string GitHubToken;
    [Parameter] readonly string NugetToken;
    [Parameter] readonly string DiscordWebhook;

    string NugetPackageSource => "https://api.nuget.org/v3/index.json";
    string GithubPackageSource => $"https://nuget.pkg.github.com/{GithubActions.GitHubRepositoryOwner}/index.json";

    bool IsOriginalRepository => Repo.Identifier == "siscodeorg/sisbase-discord.net";
    bool IsDevelopBranch => GitVersion.BranchName == DevelopBranch;

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

    Target Publish => _ => _
        .Executes(() => {

        });
}
