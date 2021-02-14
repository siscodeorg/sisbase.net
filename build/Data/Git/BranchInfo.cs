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
        public BranchKind Kind { get; set; }
    }
}
