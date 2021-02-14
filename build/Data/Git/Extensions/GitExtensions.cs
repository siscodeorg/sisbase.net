using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Data.Git.Enums;

namespace Data.Git.Extensions {
    public static class GitExtensions {
        public static bool IsFeatureBranch(this string BranchName)
            => BranchName.Contains($"feature/");
        public static bool IsFixBranch(this string BranchName)
            => BranchName.Contains("fix/") || BranchName.Contains("bug/");
        public static bool HasCustomTag(this string BranchName)
            => BranchName.Contains("/");

        public static string GetCustomTag(this string BranchName)
            => BranchName.Split("/")[0];
        public static string GetBranchName(this string BranchName)
            => string.Join("/", BranchName.Split("/")[1..]);

        public static BranchInfo ToBranchInfo(this string BranchName) {
            Regex fixRegex = new("[0-9]+-");

            if (IsFixBranch(BranchName)) return new(BranchName[4..], BranchKind.FIX);
            if (IsFeatureBranch(BranchName)) return new(BranchName[8..], BranchKind.FEATURE);

            if (fixRegex.IsMatch(BranchName))
                return new(fixRegex.Replace(BranchName, "", 1), BranchKind.ISSUE_FIX, int.Parse(fixRegex.Match(BranchName).Value[..^1]));

            if (HasCustomTag(BranchName))
                return new(GetBranchName(BranchName), BranchKind.CUSTOM_TAG, GetCustomTag(BranchName)); ;

            return new BranchInfo(BranchName, BranchKind.UNCATEGORIZED);
        }
    }
}
