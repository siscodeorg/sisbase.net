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
    Target Announce => _ => _
    .TriggeredBy(Publish)
    .DependsOn(GetBranchInfo)
    .Requires(() => DiscordWebhook)
    .Executes( async () => {
        DiscordWebhookClient client = new(DiscordWebhook);

        EmbedBuilder embed = new();
        AddBranchInfo(embed);

        if (IsDevelopBranch) {
            embed
            .WithAuthor("[sisbase-d.net] New BETA version released!", null, Repo.HttpsUrl)
            .WithDescription($"Version : {NextGithubVersion}")
            .WithColor(Color.Red);
            embed.WithFooter("This is a beta version, should be used for testing only. Don't run this in production unless recommended by the dev team.");

        } else {
            embed
            .WithAuthor("[sisbase-d.net] New version released!", null, Repo.HttpsUrl)
            .WithDescription($"Version : [{FileVersion}](https://www.nuget.org/packages/sisbase/)")
            .WithColor(Color.Green);
        }
        await client.SendMessageAsync(embeds: new[] { embed.Build() });
    });


    void AddBranchInfo (EmbedBuilder embed) {
        if (Branches[BranchKind.PULL_REQUEST].Any()) {
            var data = Branches[BranchKind.PULL_REQUEST]
                .Select(x => $"`{x.Name}` - [#{x.PullRequestID}]({Repo.HttpsUrl}/pull/{x.PullRequestID})");
            embed.AddField("⭐ Pull Requests", string.Join("\n", data),true);
        }

        if (Branches[BranchKind.FEATURE].Any()) {
            var data = Branches[BranchKind.FEATURE]
                .Select(x => x.Name);
            embed.AddField("📦 Features", string.Join("\n", data), true);
        }

        if (Branches[BranchKind.FIX].Any() || Branches[BranchKind.ISSUE_FIX].Any()) {
            var fixes = Branches[BranchKind.FIX]
                .Select(x => x.Name);
            var issues = Branches[BranchKind.ISSUE_FIX]
                .Select(x => $@"{x.Name} [(#{x.IssueID})]({Repo.HttpsUrl}/issue/{x.IssueID})");
            var data = fixes.Concat(issues);
            embed.AddField("🐞 Bugfixes", string.Join("\n", data), true);
        }

        if (Branches[BranchKind.UNCATEGORIZED].Any()) {
            var data = Branches[BranchKind.UNCATEGORIZED].Select(x => x.Name);
            embed.AddField("📑 Uncategorized", string.Join("\n", data), true);
        }

        if (Branches[BranchKind.CUSTOM_TAG].Any()) {
            var customTags = Categorize(Branches[BranchKind.CUSTOM_TAG]);

            foreach (var (tag, branches) in customTags) {
                embed.AddField(tag.Capitalize(), string.Join("\n", branches),true);
            }
        }
    }

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

