using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Data.Git.Enums;

namespace Data.Git {
    public class BranchInfo {
        public string Name { get; set; }
        public string Tag { get; set; }
        public int IssueID { get; set; }
        public int PullRequestID { get; set; }
        public BranchKind Kind { get; set; }

        public BranchInfo(string branch, BranchKind kind, int issue) {
            Name = branch;
            Kind = kind;
            IssueID = issue;
        }

        public BranchInfo(string branch, BranchKind kind) {
            Name = branch;
            Kind = kind;
            IssueID = 0;
        }
        public BranchInfo(string branch, BranchKind kind, string tag) {
            Name = branch;
            Kind = kind;
            IssueID = 0;
            Tag = tag;
        }
    }
}
