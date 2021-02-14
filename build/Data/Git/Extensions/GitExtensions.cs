using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Git.Extensions {
    public static class GitExtensions {
        public static bool IsFeatureBranch(this string BranchName)
            => BranchName.Contains($"feature/");
        public static bool IsFixBranch(this string BranchName)
            => BranchName.Contains("fix/") || BranchName.Contains("bug/");
    }
}
