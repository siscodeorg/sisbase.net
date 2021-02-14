using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Data.Git.Enums;

namespace Data.Git {
    public class CommitInfo {
        public string Branch { get; set; }
        public int PullRequestID { get; set; }
        public CommitKind Kind { get; set; }
    }
}
