using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sisbase.Common {
    public class SisbaseResult {
        public bool IsSucess { get; init; }
        public string Error { get; init; }

        internal SisbaseResult(bool isSucess, string error) {
            IsSucess = isSucess;
            Error = error;
        }

        public static SisbaseResult FromSucess() => new(true, "");
        public static SisbaseResult FromError(string errorMessage) => new(false, errorMessage);
    }
}
