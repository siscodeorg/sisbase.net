using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using Nuke.Common.Utilities;
using Nuke.Common.Utilities.Collections;

using SisbaseBuildTools;

using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[GitHubActions("publish",
        GitHubActionsImage.UbuntuLatest,
        OnPushBranches = new [] {"beta", "release"},
        InvokedTargets = new [] {
            nameof(Publish)
        },
        ImportSecrets = new [] {
            nameof(DiscordWebhook),
            nameof(NugetToken)
        },
        ImportGitHubTokenAs = nameof(GithubToken),
        PublishArtifacts = false
)]

[GitHubActions("alpha",
        GitHubActionsImage.UbuntuLatest,
        OnPushBranches = new []{"alpha"},
        InvokedTargets = new [] {
            nameof(Pack)
        },
        ImportGitHubTokenAs = nameof(GithubToken)
)]
partial class Build : NukeBuild {

    [CI] readonly GitHubActions GithubActions;

    [Parameter] readonly string GithubToken;
    [Parameter] readonly string NugetToken;
    [Parameter] readonly string DiscordWebhook;

    string NugetPackageSource => "https://api.nuget.org/v3/index.json";
    string GithubPackageSource => $"https://nuget.pkg.github.com/siscodeorg/index.json";

    bool IsOriginalRepository => Repo.Identifier == "siscodeorg/sisbase-discord.net";
    bool IsDevelopBranch => GitVersion.BranchName == DevelopBranch;
    
    private string CurrentGithubVersion;
    private string NextGithubVersion = "9.9.9-dev9999";
    Target Pack => _ => _
        .DependsOn(Clean, Compile)
        .Produces(ArtifactsPath / "*.nupkg")
        .Executes(() => {
            DotNetPack(_ => _
                .SetProject(Solution.GetProject("sisbase"))

                .SetConfiguration(Configuration.Release)

                .SetAssemblyVersion(AssemblyVersion)
                .SetFileVersion(IsDevelopBranch ? NextGithubVersion : FileVersion )
                .SetInformationalVersion(InformationalVersion)
                
                .SetVersion(GitVersion.BranchName switch
                {
                    DevelopBranch => NextGithubVersion,
                    ReleaseBranch => AssemblyVersion,
                    _ => FileVersion
                })
                
                .SetRepositoryType("git")
                .SetRepositoryUrl("https://github.com/siscodeorg/sisbase-discord.net")
                
                .SetOutputDirectory(ArtifactsPath)
            );
        });


    Target GetNextGithubVersion => _ => _
        .Requires(() => IsOriginalRepository)
        .Requires(() => !NugetToken.IsNullOrEmpty())
        .Requires(() => !GithubToken.IsNullOrEmpty())
        .Requires(() => GitVersion.BranchName == DevelopBranch || GitVersion.BranchName == ReleaseBranch)
        .Executes(async () => {
            var latestPackageVersion = await SisbasePackageResolver.GetLatestGithubPackageVersion("siscodeorg","sisbase.net",GithubActions.GitHubActor,GithubToken);
            if (latestPackageVersion != null) {
                CurrentGithubVersion = latestPackageVersion;
                NextGithubVersion = Increment(CurrentGithubVersion);
            }

            Logger.Info($"CurrentGithubVersion : {CurrentGithubVersion}");
            Logger.Info($"NextGithubVersion : {NextGithubVersion}");
        })
    ;

    private static string Increment(string latestGithubPackages) {
        Regex intRegex = new(@$"-beta\d+");
        var id = int.Parse(intRegex.Match(latestGithubPackages).Value[5..]);
        id++;
        return intRegex.Replace(latestGithubPackages, $"-beta{id:0000}");
    }

    Target Publish => _ => _
        .DependsOn(GetNextGithubVersion,Pack)
        .Requires(() => IsOriginalRepository)
        .Requires(() => !NugetToken.IsNullOrEmpty() || !IsOriginalRepository)
        .Requires(() => !GithubToken.IsNullOrEmpty() || !IsOriginalRepository)
        .Requires(() => GitVersion.BranchName == DevelopBranch || GitVersion.BranchName == ReleaseBranch)
        .Executes(() => {
            if(IsDevelopBranch && !GithubToken.IsNullOrEmpty()) {
                DotNetNuGetAddSource(_ => _
                    .SetSource(GithubPackageSource)
                    .SetUsername(GithubActions.GitHubActor)
                    .SetPassword(GithubToken)
                    .SetName("github")
                    .EnableStorePasswordInClearText()
                );
            }
            if (IsDevelopBranch) {
                //Release to Github.
                DotNetNuGetPush(_ => _
                    .SetSource("github")
                    .CombineWith(ArtifactsPath.GlobFiles("*.nupkg"), (_, v) => _
                        .SetTargetPath(v)
                    )
                );
            } else {
                DotNetNuGetPush(_ => _
                   .SetSource(NugetPackageSource)
                   .SetApiKey(NugetToken)
                   .CombineWith(ArtifactsPath.GlobFiles("*.nupkg"), (_, v) => _
                       .SetTargetPath(v))
               );
            }
        });
}
