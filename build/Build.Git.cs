using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Data.Git;
using static Data.Git.Extensions.GitExtensions;

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
using Data.Git.Enums;

partial class Build : NukeBuild {
    [PathExecutable] readonly Tool Git;

    const string DevelopBranch = "beta";
    const string AlphaBranch = "alpha";
    const string ReleaseBranch = "release";

    const string FeaturePrefix = "feature";

    List<CommitInfo> Commits;

    Dictionary<BranchKind, List<BranchInfo>> Branches = new Dictionary<BranchKind, List<BranchInfo>> {
        [BranchKind.CUSTOM_TAG] = new(),
        [BranchKind.FEATURE] = new(),
        [BranchKind.FIX] = new(),
        [BranchKind.ISSUE_FIX] = new(),
        [BranchKind.PULL_REQUEST] = new(),
        [BranchKind.UNCATEGORIZED] = new()
    };

    Target GetCommitInfo => _ => _
        .Executes(() => {
            var releaseBranchExists = CheckForBranch(ReleaseBranch);
            if (releaseBranchExists) {
                Logger.Info($"{ReleaseBranch} Branch Found!");
                Commits = Git($"log {ReleaseBranch}.. --merges --pretty=\"%s\"").Select(x => x.Text.ToCommitInfo()).ToList();
            } else {
                Logger.Warn($"{ReleaseBranch} Branch not found. Parsing all merges");
                Commits = Git("log --merges --pretty=\"%s\"").Select(x => x.Text.ToCommitInfo()).ToList();
            }

            Logger.Info($"Commits loaded. Count : {Commits.Count} with \n" +
               $"- {Commits.Where(x => x.Kind == CommitKind.MERGE).Count()} Merges \n" +
               $"- {Commits.Where(x => x.Kind == CommitKind.PULL_REQUEST).Count()} PRs\n");
        });

    Target GetBranchInfo => _ => _
        .DependsOn(GetCommitInfo)
        .Executes(() => {
            foreach (var commit in Commits) {
                if(commit.Kind == CommitKind.PULL_REQUEST) {
                    BranchInfo branch = new(commit.Branch,BranchKind.PULL_REQUEST);
                    branch.PullRequestID = commit.PullRequestID;
                    Branches[BranchKind.PULL_REQUEST].Add(branch);
                } else {
                    var branch = commit.Branch.ToBranchInfo();
                    Branches[branch.Kind].Add(branch);
                }
            }

            Logger.Info($"Branches loaded.\n" +
               $"- {Branches[BranchKind.CUSTOM_TAG].Count()} Custom \n" +
               $"- {Branches[BranchKind.PULL_REQUEST].Count()} PRs\n" +
               $"- {Branches[BranchKind.FEATURE].Count()} Features\n" +
               $"- {Branches[BranchKind.FIX].Count()} Bugfixes\n" +
               $"- {Branches[BranchKind.ISSUE_FIX].Count()} Issues Fixed\n" +
               $"- {Branches[BranchKind.UNCATEGORIZED].Count()} Miscelaneous");
        });

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

